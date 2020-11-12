using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    /// <summary>
    /// Модель изменения информации об аккаунте
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        [MaxLength(100, ErrorMessage = "Длина логина может быть максимум 100 символов")]
        public string Username { get; set; }

        /// <summary>
        /// Пароль аккаунта
        /// </summary>
        [MaxLength(100, ErrorMessage = "Длина пароля может быть максимум 100 символов")]
        public string Password { get; set; }
    }
}
