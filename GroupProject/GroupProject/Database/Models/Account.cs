using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Database.Models
{
    public class Account
    {
        public int Id { get; set; }

        public int? PersonId { get; set; }

        public Person Person { get; set; }

        public int? RoleId { get; set; }

        public Role Role { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public List<DoctorPatient> LinkedAccounts { get; set; }
    }
}
