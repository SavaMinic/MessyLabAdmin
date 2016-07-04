using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessyLabAdmin.Models
{
    public class AssignmentTestResult
    {
        public int ID { get; set; }

        public int AssignmentTestID { get; set; }
        public virtual AssignmentTest AssignmentTest { get; set; }

        public int SolutionID { get; set; }
        public virtual Solution Solution { get; set; }

        public string CalculatedOutput { get; set; }

        public bool IsSuccess { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreatedTime { get; set; }
    }
}
