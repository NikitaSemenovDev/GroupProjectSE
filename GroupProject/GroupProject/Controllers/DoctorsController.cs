using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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



        [HttpPost("doctor/patients/{id:int}/images")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdoadImage(IFormFile image, int id)
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

                ImageProcessingResult result = new ImageProcessingResult()
                {
                    Account = await Context.Accounts.FirstOrDefaultAsync(a => a.Id == id),
                    ProcessingDateTime = imageProcessingResult.ProcessingDateTime,
                    Image = imageProcessingResult.Image,
                    ProcessingResult = JsonConvert.SerializeObject(imageProcessingResult.ProcessingResult)
                };
                Context.Add(result);
                await Context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
        }
    }
}
