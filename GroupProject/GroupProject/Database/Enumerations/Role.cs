using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Database.Enumerations
{
    /// <summary>
    /// Роли пользователей системы
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Пациент
        /// </summary>
        Patient = 1,

        /// <summary>
        /// Доктор
        /// </summary>
        Doctor = 2,

        /// <summary>
        /// Администратор
        /// </summary>
        Administrator = 3
    }
}
