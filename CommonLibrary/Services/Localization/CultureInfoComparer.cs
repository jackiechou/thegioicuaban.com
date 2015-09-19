using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;

namespace CommonLibrary.Services.Localization
{
    public class CultureInfoComparer : IComparer
    {
        private string _compare;
        public CultureInfoComparer(string compareBy)
        {
            _compare = compareBy;
        }
        public int Compare(object x, object y)
        {
            switch (_compare)
            {
                case "English":
                    return ((CultureInfo)x).EnglishName.CompareTo(((CultureInfo)y).EnglishName);
                case "Native":
                    return ((CultureInfo)x).NativeName.CompareTo(((CultureInfo)y).NativeName);
                default:
                    return ((CultureInfo)x).Name.CompareTo(((CultureInfo)y).Name);
            }
        }
    }
}
