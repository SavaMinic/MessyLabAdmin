using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNet.Mvc;
using System.Text;

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

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        public string InitialPassword { get; set; }

        public long LastLoginTimestamp { get; set; }

        [NotMapped]
        [Display(Name = "Student ID")]
        public string StudentIdentification
        {
            get
            {
                return EnrollmentYear + "/" + EnrollmentNumber;
            }
        }

        [NotMapped]
        public string SessionID
        {
            get
            {
                return LastLoginTimestamp + "" + ID;
            }
        }

        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }

        public virtual ICollection<Action> Actions { get; set; }

        [NotMapped]
        public string DefaultUsername
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(LastName.ElementAt(0));
                sb.Append(FirstName.ElementAt(0));
                sb.Append(EnrollmentYear.ToString().PadLeft(4, '0').Substring(2,2));
                sb.Append(EnrollmentNumber.ToString().PadLeft(4, '0'));
                sb.Append('d');
                return sb.ToString();
            }
        }
    }
}
