﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupProjectSE.ExternalServices;
using GroupProjectSE.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GroupProjectSE.Controllers
{
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
        [Route("get-result")]
        [HttpPost]
        public async Task<IEnumerable<double>> GetImageProcessingResult(IFormFile image)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                await image.CopyToAsync(stream);
                var byts = stream.ToArray();
                var result = await ImageProcessorService.GetImageResult(byts);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return new List<double>();
            }
        }
    }
}