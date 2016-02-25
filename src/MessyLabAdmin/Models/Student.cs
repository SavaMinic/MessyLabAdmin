using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessyLabAdmin.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int EntryYear { get; set; }
        public int EntryNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
