using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupProject.ActionResults;
using GroupProject.Database;
using GroupProject.Database.Enumerations;
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
        /// Получение пациентов доктора
        /// </summary>
        /// <returns>Список пациентов доктора</returns>
        /// <response code="200">Успешное получение списка пациентов доктора</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("doctor/patients")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatients()
        {
            try
            {
                var currentUser = await Context.Accounts.AsNoTracking()
                    .Include(a => a.LinkedAccounts)
                    .ThenInclude(a => a.LinkedAccount)
                    .ThenInclude(a => a.Person)
                    .FirstAsync(a => a.Username == User.Identity.Name);

                var patients = currentUser.LinkedAccounts
                    .Select(a => a.LinkedAccount);

                var models = patients.Select(p =>
                {
                    Patient patient = p.Person as Patient;
                    return new PatientModel()
                    {
                        AccountId = p.Id,
                        FirstName = patient.FirstName,
                        Surname = patient.Surname,
                        Patronym = patient.Patronym,
                        Email = patient.Email,
                        MedicalRecordNumber = patient.MedicalRecordNumber
                    };
                });

                var response = new
                {
                    data = models
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
                    Size = JsonConvert.SerializeObject(imageProcessingResult.Size),
                    RegionsPredictions = JsonConvert.SerializeObject(imageProcessingResult.RegionsPredictions),
                    ImagePredictions = JsonConvert.SerializeObject(imageProcessingResult.ImagePredictions),
                    DiseasesNames = JsonConvert.SerializeObject(imageProcessingResult)
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


        /// <summary>
        /// Получение всех докторов
        /// </summary>
        /// <returns>Список докторов</returns>
        /// <response code="200">Успешное получение списка докторов</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("doctors")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDoctors()
        {
            try
            {
                var doctors = await Context.Accounts.AsNoTracking()
                    .Where(a => a.RoleId == (int)Database.Enumerations.Role.Doctor)
                    .Include(a => a.Person)
                    .ToListAsync();

                var models = doctors.Select(d =>
                {
                    Doctor doctor = d.Person as Doctor;
                    return new DoctorModel()
                    {
                        AccountId = d.Id,
                        FirstName = doctor.FirstName,
                        Surname = doctor.Surname,
                        Patronym = doctor.Patronym,
                        Email = doctor.Email,
                        WorkExperience = doctor.WorkExperience
                    };
                });

                var response = new
                {
                    data = models
                };

                return new ProjectJsonResult(response);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
