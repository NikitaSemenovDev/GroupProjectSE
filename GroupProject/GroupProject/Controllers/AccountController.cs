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
using GroupProject.Database.Models;

namespace GroupProject.Controllers
{
    /// <summary>
    /// Работа с аккаунтом
    /// </summary>
    [Produces("application/json")]
    [Route("api/account")]
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
        /// Регистрация пользователя
        /// </summary>
        /// <param name="model">Модель регистрации пользователя</param>
        /// <returns>Результат регистрации</returns>
        /// <response code="201">Пользователь зарегистрирован</response>
        /// <response code="400">1. Модель регистрации некорректна.
        /// 2. Пользователь с таким логином уже существует.</response>
        /// <response code="500">Ошибка на сервере</response>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                Person personToCheck = await Context.People.FirstOrDefaultAsync(p => p.Email == model.Email);

                if (personToCheck != null)
                {
                    return BadRequest(new { Error = "Пользователь с такой электронной почтой уже зарегистрирован" });
                }

                Account accountToCheck = await Context.Accounts.FirstOrDefaultAsync(u => u.Username == model.Username);

                if (accountToCheck != null)
                {
                    return BadRequest(new { Error = "Пользователь с таким логином уже зарегистрирован!" });
                }

                Person user = new Person()
                {
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    Patronym = model.Patronym,
                    Email = model.Email
                };

                Context.Add(user);
                await Context.SaveChangesAsync();

                Account account = new Account()
                {
                    Username = model.Username,
                    Password = model.Password,
                    Person = user,
                    Role = await Context.Roles.FirstOrDefaultAsync(r => r.Id == (int)PersonRole.User)
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
        [HttpPost("login")]
        [AllowAnonymous]
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
    }
}
