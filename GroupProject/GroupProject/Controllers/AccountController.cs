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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DatabaseUser = GroupProject.Database.Models.User;

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
        /// <response code="400">Модель регистрации некорректна</response>
        /// <response code="500">Ошибка на сервере</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                DatabaseUser user = new DatabaseUser()
                {
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    Patronym = model.Patronym,
                    Username = model.Username,
                    Password = model.Password
                };

                Context.Add(user);
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
                    username = identity.Name
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
                DatabaseUser user = await Context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
                if (user == null)
                {
                    return null;
                }

                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "user")
                };

                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return new ClaimsIdentity();
            }
        }
    }
}
