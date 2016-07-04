using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessyLabAdmin.Models
{
    public class AssignmentTest : IComparable<AssignmentTest>, IEquatable<AssignmentTest>
    {
        public int ID { get; set; }

        public string GivenInput { get; set; }

        public string ExpectedOutput { get; set; }

        public int AssignmentVariantID { get; set; }
        public virtual AssignmentVariant AssignmentVariant { get; set; }

        public int CompareTo(AssignmentTest other)
        {
            return ID.CompareTo(other.ID);
        }

        public bool Equals(AssignmentTest other)
        {
            return ID == other.ID;
        }
    }
}
