using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupProject.ActionResults;
using GroupProject.Database;
using GroupProject.Database.Models;
using GroupProject.ExternalServices;
using GroupProject.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GroupProject.Controllers
{
    /// <summary>
    /// Работа с докторами
    /// </summary>
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private ILogger Logger { get; }
        private DatabaseContext Context { get; }
        private ImageProcessorService ImageProcessorService { get; }

        public DoctorsController(ILogger logger, DatabaseContext context, ImageProcessorService imageProcessorService)
        {
            Logger = logger;
            Context = context;
            ImageProcessorService = imageProcessorService;
        }


        /// <summary>
        /// Загрузка изображения пациента
        /// </summary>
        /// <param name="image">Изображение для загрузки</param>
        /// <param name="id">Идентификатор аккаунта пациента</param>
        /// <returns>Результат загрузки изображения</returns>
        /// <response code="200">Изображение загружено</response>
        /// <response code="400">1. Не удалось получить изображение
        /// 2. Указанный аккаунт не существует
        /// 3. Указанный аккаунт принадлежит не пациенту</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("doctor/patients/{id:int}/images")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdoadImage(IFormFile image, int id)
        {
            try
            {
                if (image == null)
                {
                    return BadRequest();
                }

                var patient = await Context.Accounts.Include(a => a.Person).FirstOrDefaultAsync(a => a.Id == id);

                if (patient == null)
                {
                    return BadRequest(new { Error = "Указанный аккаунт не существует." });
                }

                if (!(patient.Person is Patient))
                {
                    return BadRequest(new { Error = "Указанный аккаунт принадлежит не пациенту." });
                }

                MemoryStream stream = new MemoryStream();
                await image.CopyToAsync(stream);
                var imageProcessingResult = await ImageProcessorService.GetImageResult(stream, image.FileName);

                ImageProcessingResult result = new ImageProcessingResult()
                {
                    Account = await Context.Accounts.FirstOrDefaultAsync(a => a.Id == id),
                    ProcessingDateTime = imageProcessingResult.ProcessingDateTime,
                    Image = imageProcessingResult.Image,
                    ProcessingResult = JsonConvert.SerializeObject(imageProcessingResult.ProcessingResult)
                };
                Context.Add(result);
                await Context.SaveChangesAsync();

                imageProcessingResult.Id = result.Id;

                return new ProjectJsonResult(imageProcessingResult);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
