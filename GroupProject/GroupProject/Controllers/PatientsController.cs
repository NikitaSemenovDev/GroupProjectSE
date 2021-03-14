using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupProject.ActionResults;
using GroupProject.Database;
using GroupProject.Database.Enumerations;
using GroupProject.Database.Models;
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
        /// <response code="400">Некорректный запрос</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("patient/images")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetImages()
        {
            try
            {
                var databaseImages = await Context.ImageProcessingResults.AsNoTracking()
                   .Where(r => r.Account.Username == User.Identity.Name)
                   .ToListAsync();

                var images = databaseImages
                    .Select(i => new ServiceImageProcessingResult()
                    {
                        Id = i.Id,
                        ProcessingDateTime = i.ProcessingDateTime,
                        Image = i.Image,
                        Size = JsonConvert.DeserializeObject<IEnumerable<int>>(i.Size),
                        RegionsPredictions = JsonConvert.DeserializeObject<IEnumerable<RegionPrediction>>(i.RegionsPredictions),
                        ImagePredictions = JsonConvert.DeserializeObject<IEnumerable<double>>(i.ImagePredictions),
                        DiseasesNames = JsonConvert.DeserializeObject<IEnumerable<Disease>>(i.DiseasesNames)
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


        /// <summary>
        /// Получение результатов обработок изображений пациента
        /// </summary>
        /// <param name="id">Идентификатор пациента</param>
        /// <returns>Результаты обработок изображений пациента</returns>
        /// <response code="200">Успешное получение результатов обработок изображений</response>
        /// <response code="400">1. Некорректный запрос
        /// 2. Аккаунт с указанным идентификатором не найден
        /// 3. Указанный аккаунт принадлежит не пользователю</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("patients/{id:int}/images")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetImages(int id)
        {
            try
            {
                var patient = await Context.Accounts.Include(a => a.Person).FirstOrDefaultAsync(a => a.Id == id);

                if (patient == null)
                {
                    return BadRequest(new { Error = "Аккаунт с указанным идентификатором не найден." });
                }

                if (!(patient.Person is Patient))
                {
                    return BadRequest(new { Error = "Указанный аккаунт принадлежит не пациенту." });
                }

                var databaseImages = await Context.ImageProcessingResults.AsNoTracking()
                    .Where(i => i.AccountId == id)
                    .ToListAsync();

                var images = databaseImages
                    .Select(i => new ServiceImageProcessingResult()
                    {
                        Id = i.Id,
                        ProcessingDateTime = i.ProcessingDateTime,
                        Image = i.Image,
                        Size = JsonConvert.DeserializeObject<IEnumerable<int>>(i.Size),
                        RegionsPredictions = JsonConvert.DeserializeObject<IEnumerable<RegionPrediction>>(i.RegionsPredictions),
                        ImagePredictions = JsonConvert.DeserializeObject<IEnumerable<double>>(i.ImagePredictions),
                        DiseasesNames = JsonConvert.DeserializeObject<IEnumerable<Disease>>(i.DiseasesNames)
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
        /// Получение докторов пациента
        /// </summary>
        /// <returns>Список докторов пациента</returns>
        /// <response code="200">Успешное получение списка докторов пациента</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("patient/doctors")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDoctors()
        {
            try
            {
                var currentUser = await Context.Accounts.AsNoTracking()
                    .Include(a => a.LinkedAccounts)
                    .ThenInclude(a => a.LinkedAccount)
                    .ThenInclude(a => a.Person)
                    .FirstAsync(a => a.Username == User.Identity.Name);

                var doctors = currentUser.LinkedAccounts
                    .Select(a => a.LinkedAccount);

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


        /// <summary>
        /// Получение всех пациентов
        /// </summary>
        /// <returns>Список пациентов</returns>
        /// <response code="200">Успешное получение списка пациентов</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("patients")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatients()
        {
            try
            {
                var patients = await Context.Accounts.AsNoTracking()
                    .Where(a => a.RoleId == (int)Database.Enumerations.Role.Patient)
                    .Include(a => a.Person)
                    .ToListAsync();

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
    }
}
