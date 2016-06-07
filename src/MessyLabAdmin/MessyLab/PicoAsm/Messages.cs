using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessyLabAdmin.MessyLab.PicoAsm
{
    public static class Messages
    {

        public static string AtLineLowerCase = "at line";
        public static string ColumnLowerCase = "column";
        public static string E0000 = "Syntax Error ({0})";
        public static string E0001 = "The symbol '{0}' is already defined";
        public static string E0002 = "The value '{0}' is out of range";
        public static string E0003 = "Address error in argument '{0}'. Legal adresses [{1}..{2}]";
        public static string E0004 = "Zero is the only constant allowed in branch instructions";
        public static string E0005 = "Allowed constant values for the I/O instructions are in range [0..127]";
        public static string E0006 = "Program is too big. Memory limit reached";
        public static string E0007 = "The symbol '{0}' is not defined";
    }
}
