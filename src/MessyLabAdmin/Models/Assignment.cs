using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessyLabAdmin.Models
{
    public class Assignment
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Unesite naslov zadatka")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Unesite opis zadatka")]
        public string Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Starting at")]
        [Required(ErrorMessage = "Unesite vreme početka")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ending at")]
        [Required(ErrorMessage = "Unesite vreme završetka")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created at")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreatedTime { get; set; }

        // For selecting students
        public int? SelectEnrollmentNumberDiv { get; set; }
        public int? SelectEnrollmentYear { get; set; }
        public int? SelectStatus { get; set; }

        public string CreatedByID;
        public virtual ApplicationUser CreatedBy { get; set; }

        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }
    }
}
