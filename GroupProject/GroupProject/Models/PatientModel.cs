using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    /// <summary>
    /// Модель пациента
    /// </summary>
    public class PatientModel
    {
        /// <summary>
        /// Идентификатор аккаунта пациента
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Имя пациента
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия пациента
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Отчество пациента
        /// </summary>
        public string Patronym { get; set; }

        /// <summary>
        /// Электронная почта
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Номер медицинской карты пациента
        /// </summary>
        public int? MedicalRecordNumber { get; set; }
    }
}
