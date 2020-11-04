using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using GroupProject.ActionResults;
using GroupProject.Database;
using GroupProject.Database.Models;
using GroupProject.ExternalServices;
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
    /// Работа с изображениями
    /// </summary>
    [Produces("application/json")]
    [Route("api/image-processing")]
    [ApiController]
    public class ImageProcessingController : ControllerBase
    {
        private ILogger Logger { get; }
        private DatabaseContext Context { get; }
        private ImageProcessorService ImageProcessorService { get; }

        public ImageProcessingController(ILogger logger, DatabaseContext context, ImageProcessorService imageProcessorService)
        {
            Logger = logger;
            Context = context;
            ImageProcessorService = imageProcessorService;
        }

        /// <summary>
        /// Получение результата обработки изображения
        /// </summary>
        /// <param name="image">Изображение для обработки</param>
        /// <returns>Результат обработки изображения</returns>
        /// <response code="200">Обработка изображения выполнилась успешно</response>
        /// <response code="400">Не удалось получить изображение</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("get-result")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetImageProcessingResult(IFormFile image)
        {
            try
            {
                if (image == null)
                {
                    return BadRequest();
                }
                MemoryStream stream = new MemoryStream();
                await image.CopyToAsync(stream);
                var imageProcessingResult = await ImageProcessorService.GetImageResult(stream, image.FileName);

                if (User != null && User.Identity.IsAuthenticated)
                {
                    ImageProcessingResult result = new ImageProcessingResult()
                    {
                        Account = await Context.Accounts.FirstAsync(a => a.Username == User.Identity.Name),
                        ProcessingDateTime = imageProcessingResult.ProcessingDateTime,
                        Image = imageProcessingResult.Image,
                        ProcessingResult = JsonConvert.SerializeObject(imageProcessingResult.ProcessingResult)
                    };
                    Context.ImageProcessingResults.Add(result);
                    await Context.SaveChangesAsync();

                    imageProcessingResult.Id = result.Id;
                }
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