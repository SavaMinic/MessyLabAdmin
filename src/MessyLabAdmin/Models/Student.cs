using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace MessyLabAdmin.Models
{
    public class Student
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Enrollment year")]
        [Range(1950, 2050, ErrorMessage = "Year should be in range 1950-2050")]
        public int EnrollmentYear { get; set; }

        [Display(Name = "Enrollment number")]
        [Range(1, 2000, ErrorMessage = "Number should be in range 1-2000")]
        public int EnrollmentNumber { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        [NotMapped]
        [Display(Name = "Student ID")]
        public string StudentID
        {
            get
            {
                return EnrollmentYear + "/" + EnrollmentNumber;
            }
        }

        public virtual ICollection<Assignment> Assignements { get; set; }

        public virtual ICollection<Solution> Solutions { get; set; }

        public virtual ICollection<Action> Actions { get; set; }
    }
}
