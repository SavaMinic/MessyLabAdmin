using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Range(0, 2000, ErrorMessage = "Number should be in range 1950-2050")]
        public int EnrollmentNumber { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public virtual ICollection<Assignment> Assignements { get; set; }

        public virtual ICollection<Solution> Solutions { get; set; }

        public virtual ICollection<Action> Actions { get; set; }
    }
}
