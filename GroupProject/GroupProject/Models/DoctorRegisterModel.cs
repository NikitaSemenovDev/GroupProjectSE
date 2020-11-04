using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    /// <summary>
    /// Модель регистрации доктора
    /// </summary>
    public class DoctorRegisterModel
    {
        /// <summary>
        /// Имя доктора
        /// </summary>
        [Required(ErrorMessage = "Введите имя доктора")]
        [MaxLength(100, ErrorMessage = "Длина имени может быть максимум 100 символов")]
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия доктора
        /// </summary>
        [Required(ErrorMessage = "Введите фамилию доктора")]
        [MaxLength(100, ErrorMessage = "Длина фамилии может быть максимум 100 символов")]
        public string Surname { get; set; }

        /// <summary>
        /// Отчество доктора
        /// </summary>
        [MaxLength(100, ErrorMessage = "Длина отчества может быть максимум 100 символов")]
        public string Patronym { get; set; }

        /// <summary>
        /// Электронная почта
        /// </summary>
        [Required(ErrorMessage = "Введите электронный ящик доктора")]
        [MaxLength(100, ErrorMessage = "Длина электронной почты может быть максимум 100 символов")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите стаж работы доктора")]
        public int? WorkExperience { get; set; }

        /// <summary>
        /// Логин доктора
        /// </summary>
        [Required(ErrorMessage = "Введите логин доктора")]
        [MaxLength(100, ErrorMessage = "Длина логина может быть максимум 100 символов")]
        public string Username { get; set; }

        /// <summary>
        /// Пароль доктора
        /// </summary>
        [Required(ErrorMessage = "Введите пароль доктора")]
        [MaxLength(100, ErrorMessage = "Длина пароля может быть максимум 100 символов")]
        public string Password { get; set; }
    }
}
