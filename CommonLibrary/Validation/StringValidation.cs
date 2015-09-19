using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Validation
{
    public class StringValidation
    {
        public bool checkValidString(string input)
        {
            const string regExpr = @"^[a-zA-Z]+$"; //regular expression for valid string
            // match input with regular expression
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(input, regExpr);
            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkAlphanumericStringValue(string input)
        {
            // This will restrict user to enter only alphanumeric value with minimun of 3 character and maximun of 30 character.
            const string regExpr = @"^[A-Za-z1-9]{3,30}$"; //regular expression for valid string
            // match input with regular expression
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(input, regExpr);
            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool checkValidEmail(string input)
        {
            const string regExpr = @"^(?("")("".+?""@)|" +
                  @"(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)" +
                  @"(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])" +
                  @"|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$";
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(input, regExpr);
            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }      
  

    }
}
