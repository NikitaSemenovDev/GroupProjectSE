using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Models
{
    /// <summary>
    /// Модель доктора
    /// </summary>
    public class DoctorModel
    {
        /// <summary>
        /// Идентификатор аккаунта доктора
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Имя доктора
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия доктора
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Отчество доктора
        /// </summary>
        public string Patronym { get; set; }

        /// <summary>
        /// Электронная почта
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Стаж
        /// </summary>
        public int? WorkExperience { get; set; }
    }
}
