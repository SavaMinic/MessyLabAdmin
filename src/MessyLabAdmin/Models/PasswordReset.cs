using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace MessyLabAdmin.Models
{
    public class PasswordReset
    {
        public int ID { get; set; }

        public int StudentID { get; set; }
        public virtual Student Student { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created at")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreatedTime { get; set; }

        [Required]
        public string RequestCode { get; set; }

        public bool IsUsed { get; set; }
    }
}
