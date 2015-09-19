using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Services.FileSystem
{
    public class FileManagerFunctions
    {
        public static string CReplace(string strExpression, string strSearch, string strReplace, int intMode)
        {
            string strReturn;
            int lngPosition;
            string strTemp;
            if (intMode == 1)
            {
                strReturn = "";
                strSearch = strSearch.ToUpper();
                strTemp = strExpression.ToUpper();
                lngPosition = strTemp.IndexOf(strSearch);
                while (lngPosition >= 0)
                {
                    strReturn = strReturn + strExpression.Substring(0, lngPosition) + strReplace;
                    strExpression = strExpression.Substring(lngPosition + strSearch.Length);
                    strTemp = strTemp.Substring(lngPosition + strSearch.Length);
                    lngPosition = strTemp.IndexOf(strSearch);
                }
                strReturn = strReturn + strExpression;
            }
            else
            {
                strReturn = strExpression.Replace(strSearch, strReplace);
            }
            return strReturn;
        }
    }
}
