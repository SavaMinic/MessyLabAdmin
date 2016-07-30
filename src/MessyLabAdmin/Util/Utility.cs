using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessyLabAdmin.Util
{
    public static class Utility
    {

        private static Random Random = new Random();

        public static string CalculatePasswordHash(string username, string pass)
        {
            using (SHA1 sha1Hash = SHA1.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(username + pass));

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        public static string CalculatePasswordRequestCode(int len = 32)
        {
            var code = new char[len];
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < len; i++)
            {
                code[i] = chars[Random.Next(chars.Length)];
            }
            return new string(code);
        }

        public static string ConvertNewLineToBR(this string s)
        {
            if (s == null || s == "") return "";
            return Regex.Replace(s, Environment.NewLine, "<br />");
        }

        public static string ConvertUrlEncodedToNewLine(this string s)
        {
            if (s == null || s == "") return "";
            return Regex.Replace(s, "%0D%0A", Environment.NewLine);
        }
    }
}
