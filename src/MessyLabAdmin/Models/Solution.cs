﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel;

namespace MessyLabAdmin.Models
{
    public class Solution
    {

        public int ID { get; set; }

        [Required]
        public string Code { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created at")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreatedTime { get; set; }
        
        public int StudentID { get; set; }
        public virtual Student Student { get; set; }
        
        public int? AssignmentID { get; set; }
        public virtual Assignment Assignment { get; set; }

        [Display(Name = "Checked and evaluated")]
        public bool IsEvaluated { get; set; }

        public bool IsHistory { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Last tested at")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime LastTestedTime { get; set; }

        public virtual ICollection<AssignmentTestResult> AssignmentTestResults { get; set; }
    }
}
