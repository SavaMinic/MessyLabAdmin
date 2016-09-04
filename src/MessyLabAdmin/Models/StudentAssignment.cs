using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MessyLabAdmin.Models
{
    public class StudentAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int StudentID { get; set; }
        public int AssignmentID { get; set; }
        public int? SolutionID { get; set; }

        public virtual Student Student { get; set; }
        public virtual Assignment Assignment { get; set; }
        public int AssignmentVariantIndex { get; set; }

        public virtual Solution Solution { get; set; }

        public virtual ICollection<Solution> SolutionHistory { get; set; }

    }
}
