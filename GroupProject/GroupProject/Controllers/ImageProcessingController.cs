using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using GroupProject.ActionResults;
using GroupProject.ExternalServices;
using GroupProject.Logging;
using GroupProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private ImageProcessorService ImageProcessorService { get; }

        public ImageProcessingController(ILogger logger, ImageProcessorService imageProcessorService)
        {
            Logger = logger;
            ImageProcessorService = imageProcessorService;
        }

        /// <summary>
        /// Получение результата обработки изображения
        /// </summary>
        /// <param name="image">Изображение для обработки</param>
        /// <returns>Результат обработки изображения</returns>
        /// <response code="200">Обработка изображения выполнилась успешно</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("get-result")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
                var result = await ImageProcessorService.GetImageResult(stream, image.FileName);
                return new ProjectJsonResult(result);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}