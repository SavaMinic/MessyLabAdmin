using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessyLabAdmin.Models
{
    public class AssignmentVariant : IComparable<AssignmentVariant>, IEquatable<AssignmentVariant>
    {
        public int ID { get; set; }

        public string Text { get; set; }

        public int AssignmentID { get; set; }
        public virtual Assignment Assignment { get; set; }

        public virtual ICollection<AssignmentTest> AssignmentTests { get; set; }

        public int CompareTo(AssignmentVariant other)
        {
            return ID.CompareTo(other.ID);
        }

        public bool Equals(AssignmentVariant other)
        {
            return ID == other.ID;
        }
    }
}
