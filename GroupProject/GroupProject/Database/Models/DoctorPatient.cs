using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Database.Models
{
    public class DoctorPatient
    {
        public int Id { get; set; }

        public int? AccountId { get; set; }

        public Account Account { get; set; }

        public int? LinkedAccountId { get; set; }

        public Account LinkedAccount { get; set; }
    }
}
