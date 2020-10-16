using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    /// <summary>
    /// Модель регистрации пользователя
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Required(ErrorMessage = "Введите имя пользователя")]
        [MaxLength(100, ErrorMessage = "Длина имени может быть максимум 100 символов")]
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        [Required(ErrorMessage = "Введите фамилию пользователя")]
        [MaxLength(100, ErrorMessage = "Длина фамилии может быть максимум 100 символов")]
        public string Surname { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        [MaxLength(100, ErrorMessage = "Длина отчества может быть максимум 100 символов")]
        public string Patronym { get; set; }

        /// <summary>
        /// Электронная почта
        /// </summary>
        [Required(ErrorMessage = "Введите электронный ящик пользователя")]
        [MaxLength(100, ErrorMessage = "Длина электронной почты может быть максимум 100 символов")]
        public string Email { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required(ErrorMessage = "Введите логин пользователя")]
        [MaxLength(100, ErrorMessage = "Длина логина может быть максимум 100 символов")]
        public string Username { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [Required(ErrorMessage = "Введите пароль пользователя")]
        [MaxLength(100, ErrorMessage = "Длина пароля может быть максимум 100 символов")]
        public string Password { get; set; }
    }
}
