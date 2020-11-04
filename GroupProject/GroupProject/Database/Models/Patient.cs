using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Database.Models
{
    public class Patient : Person
    {
        public int MedicalRecordNumber { get; set; }
    }
}
