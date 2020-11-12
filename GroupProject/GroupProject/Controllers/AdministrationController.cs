using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GroupProject.Database;
using GroupProject.Database.Enumerations;
using GroupProject.Database.Models;
using GroupProject.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonRole = GroupProject.Database.Enumerations.Role;

namespace GroupProject.Controllers
{
    /// <summary>
    /// Методы для выполнения администратором
    /// </summary>
    [Produces("application/json")]
    [Route("api/administration")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class AdministrationController : ControllerBase
    {
        private ILogger Logger { get; }
        private DatabaseContext Context { get; }

        
        public AdministrationController(ILogger logger, DatabaseContext context)
        {
            Logger = logger;
            Context = context;
        }


        /// <summary>
        /// Добавление связи между врачом и пациентом
        /// </summary>
        /// <param name="doctorAccountId">Идентификатор аккаунта врача</param>
        /// <param name="patientAccountId">Идентификатор аккаунта пациента</param>
        /// <returns>Результат добавления связи между пациентом и врачом</returns>
        /// <response code="204">Успешное добавление связи</response>
        /// <response code="400">1. Аккаунт врача с указанным идентификатором не найден
        /// 2. Аккаунт пациента с указанным идентификатором не найден
        /// 3.Связь между указанными пациентом и врачом уже существует</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("connect")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Connect([Required]int doctorAccountId, [Required]int patientAccountId)
        {
            try
            {
                var accounts = Context.Accounts.AsNoTracking();

                var doctor = await accounts.FirstOrDefaultAsync(a => a.Id == doctorAccountId && a.RoleId == (int)PersonRole.Doctor);

                if (doctor == null)
                {
                    return BadRequest(new { error = "Аккаунт врача с указанным идентификатором не найден" });
                }

                var patient = await accounts.FirstOrDefaultAsync(a => a.Id == patientAccountId && a.RoleId == (int)PersonRole.Patient);

                if (patient == null)
                {
                    return BadRequest(new { error = "Аккаунт пациента с указанным идентификатором не найден" });
                }

                var connection = await Context.DoctorPatients.AsNoTracking()
                    .FirstOrDefaultAsync(dp => dp.AccountId == doctorAccountId && dp.LinkedAccountId == patientAccountId);

                if (connection != null)
                {
                    return BadRequest(new { error = "Связь между указанными пациентом и врачом уже существует" });
                }

                DoctorPatient doctorPatient = new DoctorPatient()
                {
                    AccountId = doctorAccountId,
                    LinkedAccountId = patientAccountId
                };
                Context.Add(doctorPatient);

                DoctorPatient patientDoctor = new DoctorPatient()
                {
                    AccountId = patientAccountId,
                    LinkedAccountId = doctorAccountId
                };
                Context.Add(patientDoctor);

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
