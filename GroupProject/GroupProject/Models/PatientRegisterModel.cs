using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    /// <summary>
    /// Модель регистрации пациента
    /// </summary>
    public class PatientRegisterModel
    {
        /// <summary>
        /// Имя пациента
        /// </summary>
        [Required(ErrorMessage = "Введите имя пациента")]
        [MaxLength(100, ErrorMessage = "Длина имени может быть максимум 100 символов")]
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия пациента
        /// </summary>
        [Required(ErrorMessage = "Введите фамилию пациента")]
        [MaxLength(100, ErrorMessage = "Длина фамилии может быть максимум 100 символов")]
        public string Surname { get; set; }

        /// <summary>
        /// Отчество пациента
        /// </summary>
        [MaxLength(100, ErrorMessage = "Длина отчества может быть максимум 100 символов")]
        public string Patronym { get; set; }

        /// <summary>
        /// Электронная почта
        /// </summary>
        [Required(ErrorMessage = "Введите электронный ящик пациента")]
        [MaxLength(100, ErrorMessage = "Длина электронной почты может быть максимум 100 символов")]
        public string Email { get; set; }

        /// <summary>
        /// Номер медицинской карты пациента
        /// </summary>
        public int? MedicalRecordNumber { get; set; }

        /// <summary>
        /// Логин пациента
        /// </summary>
        [Required(ErrorMessage = "Введите логин пациента")]
        [MaxLength(100, ErrorMessage = "Длина логина может быть максимум 100 символов")]
        public string Username { get; set; }

        /// <summary>
        /// Пароль пациента
        /// </summary>
        [Required(ErrorMessage = "Введите пароль пациента")]
        [MaxLength(100, ErrorMessage = "Длина пароля может быть максимум 100 символов")]
        public string Password { get; set; }
    }
}
