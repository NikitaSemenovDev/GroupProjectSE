﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupProject.ActionResults;
using GroupProject.Database;
using GroupProject.Logging;
using GroupProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GroupProject.Controllers
{
    /// <summary>
    /// Работа с пациентами
    /// </summary>
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private ILogger Logger { get; }
        private DatabaseContext Context { get; }


        public PatientsController(ILogger logger, DatabaseContext context)
        {
            Logger = logger;
            Context = context;
        }



        /// <summary>
        /// Получение истории обработок изображений
        /// </summary>
        /// <returns>История обработок изображений</returns>
        /// <response code="200">История обработок изображений получена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("patient/images")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetImages()
        {
            try
            {
                var databaseImages = await Context.ImageProcessingResults.AsNoTracking()
                   .Where(r => r.Account.Username == User.Identity.Name)
                   .Select(r => new
                   {
                       Id = r.Id,
                       ProcessingDateTime = r.ProcessingDateTime,
                       Image = r.Image,
                       ProcessingResult = r.ProcessingResult
                   })
                   .ToListAsync();

                var images = databaseImages
                    .Select(i => new ServiceImageProcessingResult()
                    {
                        Id = i.Id,
                        ProcessingDateTime = i.ProcessingDateTime,
                        Image = i.Image,
                        ProcessingResult = JsonConvert.DeserializeObject<double[]>(i.ProcessingResult)
                    });

                var response = new
                {
                    data = images
                };

                return new ProjectJsonResult(response);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Удаление результата обработки изображения
        /// </summary>
        /// <param name="id">Идентификатор результата обработки изображения, который необходимо удалить</param>
        /// <returns>Результат удаления результата обработки изображения</returns>
        /// <response code="204">Удаление успешно выполнено</response>
        /// <response code="400">Результат обработки изображения не найден</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpDelete("patient/images/{id:int}")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var imageToDelete = await Context.ImageProcessingResults
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (imageToDelete == null)
                {
                    return BadRequest(new { error = "Результат обработки изображения с указанным идентификатором не найден." });
                }

                Context.ImageProcessingResults.Remove(imageToDelete);
                await Context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}