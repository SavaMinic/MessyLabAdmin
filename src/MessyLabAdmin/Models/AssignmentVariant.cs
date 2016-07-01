using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessyLabAdmin.Models
{
    public class AssignmentVariant
    {
        public int ID { get; set; }

        public string Text { get; set; }

        public int AssignmentID { get; set; }
        public virtual Assignment Assignment { get; set; }
    }
}
