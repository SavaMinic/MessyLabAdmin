using System;
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
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy t}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy t}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        public ApplicationUser CreatedBy { get; set; }
    }
}
