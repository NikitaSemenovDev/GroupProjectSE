using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Database.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Patronym { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
