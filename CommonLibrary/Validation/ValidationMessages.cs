using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Validation
{
    public class ValidationMessages
    {
        public static readonly string Url = "* Please enter a valid URL.<br>Valid " + 
           "characters are all alphanumeric characters and .?" + 
           "&_=-$<br> example: home.aspx?id=5&name=$my_name";
        public static readonly string Required = "* Required";
        public static readonly string Date = 
           "* Please enter a valid date in dd-MM-yyyy format.";
        public static readonly string Time = 
           "* Please enter a valid time in hh:mm:ss am format.";
        public static readonly string Number = "* Must be a valid number.";
        public static readonly string Digit = "* Must be a valid whole number.";
        public static readonly string NonNegative = "* Must be a non-negative number.";

        public static string MaxLength(int len)
        {
            return "* Maximum " + len.ToString() + 
                   " characters are allowed.";
        }
    }    
}
