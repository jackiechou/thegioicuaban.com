using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Modules
{
    public class MathClass
    {
        public static bool IsNumeric(object expression)
        {
            if (expression == null)
                return false;

            double inputDouble;

            if (double.TryParse(expression.ToString(), out inputDouble))
                return true;

            bool inputBool;
            if (bool.TryParse(expression.ToString(), out inputBool))
                return true;

            return false;
        }

        public bool CheckLengthOfNumberInput(string strInput, int numeric_length)
        {

            bool result = false;
            if (strInput != null)
            {
                string pattern = @"^\d{" + numeric_length + "}$";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result = regex.IsMatch(strInput);
            }
            return result;

        }

        public bool CheckNumberInputLetters(string strInput, int strLength)
        {
            bool result = false;
            if (strInput != null)
            {
                string pattern = @"\w{0," + strLength + "}";
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);
                result = regex.IsMatch(strInput);
            }
            return result;
        }
    }
}
