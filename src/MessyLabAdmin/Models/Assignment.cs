using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MessyLabAdmin.Models
{
    public class Assignment
    {
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Starting at")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy t}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ending at")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy t}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created at")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy t}", ApplyFormatInEditMode = true)]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "Created by")]
        public virtual ApplicationUser CreatedBy { get; set; }

        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }
    }
}
