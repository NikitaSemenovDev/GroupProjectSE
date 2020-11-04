using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Database.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Patronym { get; set; }

        public string Email { get; set; }

        public List<Account> Accounts { get; set; }
    }
}
