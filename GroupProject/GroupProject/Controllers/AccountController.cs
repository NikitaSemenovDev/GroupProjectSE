using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using GroupProject.AuthorizationAuthentication;
using GroupProject.Database;
using GroupProject.Logging;
using GroupProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonRole = GroupProject.Database.Enumerations.Role;
using DatabaseImageProcessingResult = GroupProject.Database.Models.ImageProcessingResult;
using GroupProject.Database.Models;
using Newtonsoft.Json;
using GroupProject.ActionResults;
using GroupProject.Database.Enumerations;

namespace GroupProject.Controllers
{
    /// <summary>
    /// Работа с аккаунтом
    /// </summary>
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private ILogger Logger { get; }
        private DatabaseContext Context { get; }


        public AccountController(ILogger logger, DatabaseContext context)
        {
            Logger = logger;
            Context = context;
        }


        /// <summary>
        /// Регистрация пацента
        /// </summary>
        /// <param name="model">Модель регистрации пациента</param>
        /// <returns>Результат регистрации</returns>
        /// <response code="201">Пациент зарегистрирован</response>
        /// <response code="400">1. Модель регистрации некорректна.
        /// 2. Пациент с таким логином уже существует.</response>
        /// <response code="500">Ошибка на сервере</response>
        [HttpPost("accounts/patients/register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(PatientRegisterModel model)
        {
            try
            {
                Account accountToCheck = await Context.Accounts.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (accountToCheck != null)
                {
                    return BadRequest(new { Error = "Пациент с таким логином уже зарегистрирован!" });
                }

                Person person = await Context.People.FirstOrDefaultAsync(p => p.Email == model.Email);

                if (person == null)
                {
                    person = new Patient()
                    {
                        FirstName = model.FirstName,
                        Surname = model.Surname,
                        Patronym = model.Patronym,
                        Email = model.Email,
                        MedicalRecordNumber = model.MedicalRecordNumber
                    };

                    Context.Add(person);
                    await Context.SaveChangesAsync();
                }

                Account account = new Account()
                {
                    Username = model.Username,
                    Password = model.Password,
                    Person = person,
                    Role = await Context.Roles.FirstOrDefaultAsync(r => r.Id == (int)PersonRole.Patient)
                };

                Context.Add(account);
                await Context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Регистрация доктора
        /// </summary>
        /// <param name="model">Модель регистрации доктора</param>
        /// <returns>Результат регистрации</returns>
        /// <response code="201">Доктор зарегистрирован</response>
        /// <response code="400">1. Модель регистрации некорректна.
        /// 2. Доктор с таким логином уже существует.</response>
        /// <response code="500">Ошибка на сервере</response>
        [HttpPost("accounts/doctors/register")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(DoctorRegisterModel model)
        {
            try
            {
                Account accountToCheck = await Context.Accounts.FirstOrDefaultAsync(a => a.Username == model.Username);

                if (accountToCheck != null)
                {
                    return BadRequest(new { Error = "Доктор с таким логином уже зарегистрирован" });
                }

                Person person = await Context.People.FirstOrDefaultAsync(p => p.Email == model.Email);

                if (person == null)
                {
                    person = new Doctor()
                    {
                        FirstName = model.FirstName,
                        Surname = model.Surname,
                        Patronym = model.Patronym,
                        Email = model.Email,
                        WorkExperience = model.WorkExperience.Value
                    };

                    Context.Add(person);
                    await Context.SaveChangesAsync();
                }

                Account account = new Account()
                {
                    Username = model.Username,
                    Password = model.Password,
                    Person = person,
                    Role = await Context.Roles.FirstOrDefaultAsync(r => r.Id == (int)PersonRole.Doctor)
                };

                Context.Add(account);
                await Context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Аутентификация пользователя в системе
        /// </summary>
        /// <param name="username">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Токен для доступа к серверу</returns>
        /// <response code="200">Пользователь аутентифицирован</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpPost("account/login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                ClaimsIdentity identity = await GetIdentity(username, password);
                
                if (identity == null)
                {
                    return BadRequest(new { error = "Неверные логин или пароль." });
                }

                DateTime now = DateTime.UtcNow;

                SigningCredentials credentials = 
                    new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);

                JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: AuthOptions.Issuer,
                    audience: AuthOptions.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.AddMinutes(AuthOptions.Lifetime),
                    signingCredentials: credentials);

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = identity.Name,
                    role = identity.Claims.FirstOrDefault(c => c.Type == identity.RoleClaimType).Value
                };

                return new JsonResult(response);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Получение идентификационной информации о пользователе
        /// </summary>
        /// <param name="username">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Идентификационная информация</returns>
        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            try
            {
                Account account = await Context.Accounts.Include(a => a.Role)
                    .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
                
                if (account == null)
                {
                    return null;
                }

                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, account.Username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, account.Role.Name)
                };

                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }
            catch (Exception e)
            {
                string exception = e.ToString();
                Logger.Error(exception);
                throw new Exception(exception);
            }
        }


        /// <summary>
        /// Получение информации об авторизованном пользователе
        /// </summary>
        /// <returns>Информация о пользователе</returns>
        /// <response code="200">Информация успешно получена</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("account")]
        [Authorize(Roles = "Patient,Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccountInformation()
        {
            try
            {
                Account account = await Context.Accounts.AsNoTracking()
                    .Include(a => a.Person)
                    .FirstAsync(a => a.Username == User.Identity.Name);

                object data = null;
                if (User.IsInRole(PersonRole.Patient.ToString()))
                {
                    Patient patient = account.Person as Patient;
                    data = new PatientModel()
                    {
                        AccountId = account.Id,
                        FirstName = patient.FirstName,
                        Surname = patient.Surname,
                        Patronym = patient.Patronym,
                        Email = patient.Email,
                        MedicalRecordNumber = patient.MedicalRecordNumber
                    };
                }
                else
                {
                    Doctor doctor = account.Person as Doctor;
                    data = new DoctorModel()
                    {
                        AccountId = account.Id,
                        FirstName = doctor.FirstName,
                        Surname = doctor.Surname,
                        Patronym = doctor.Patronym,
                        Email = doctor.Email,
                        WorkExperience = doctor.WorkExperience
                    };
                }

                return new ProjectJsonResult(data);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Получение информации о пользователе
        /// </summary>
        /// <param name="id">Идентификатор аккаунта пользователя</param>
        /// <returns>Информация о пользователе</returns>
        /// <response code="200">Информация успешно получена</response>
        /// <response code="400">Аккаунта с переданным идентификатором не найдено</response>
        /// <response code="500">Ошибка сервера</response>
        [HttpGet("accounts/{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccountInformation(int id)
        {
            try
            {
                Account account = await Context.Accounts.AsNoTracking()
                    .Include(a => a.Person)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (account == null)
                {
                    return BadRequest(new { error = "Аккаунта с переданным идентификатором не найдено." });
                }

                object data = null;
                if (account.Person is Patient)
                {
                    Patient patient = account.Person as Patient;
                    data = new PatientModel()
                    {
                        AccountId = account.Id,
                        FirstName = patient.FirstName,
                        Surname = patient.Surname,
                        Patronym = patient.Patronym,
                        Email = patient.Email,
                        MedicalRecordNumber = patient.MedicalRecordNumber
                    };
                }
                else
                {
                    Doctor doctor = account.Person as Doctor;
                    data = new DoctorModel()
                    {
                        AccountId = account.Id,
                        FirstName = doctor.FirstName,
                        Surname = doctor.Surname,
                        Patronym = doctor.Patronym,
                        Email = doctor.Email,
                        WorkExperience = doctor.WorkExperience
                    };
                }

                return new ProjectJsonResult(data);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
