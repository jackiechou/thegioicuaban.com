using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibrary.Modules
{
    public class AgeClass
    {       
        public struct Age
        {
            public readonly int Years;
            public readonly int Months;
            public readonly int Days;

            public Age(int y, int m, int d)
            {
                Years = y;
                Months = m;
                Days = d;
            }

            public static Age CalculateAge(string StartDate, string EndDate)
            {
                System.Globalization.DateTimeFormatInfo dateInfo = new System.Globalization.DateTimeFormatInfo();
                dateInfo.ShortDatePattern = "yyyy-MM-dd";
                IFormatProvider culture = new System.Globalization.CultureInfo("en-GB", true);
                DateTime startDate = DateTime.Parse(StartDate, culture, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                DateTime endDate = DateTime.Parse(EndDate, culture, System.Globalization.DateTimeStyles.NoCurrentDateDefault);
                                
                //if (startDate.Date > endDate.Date)
                //{
                //    throw new ArgumentException("startDate cannot be higher then endDate", "startDate");
                //}

                int years = endDate.Year - startDate.Year;
                int months = 0;
                int days = 0;

                // Check if the last year, was a full year.
                if (endDate < startDate.AddYears(years) && years != 0)
                {
                    years--;
                }

                // Calculate the number of months.
                startDate = startDate.AddYears(years);

                if (startDate.Year == endDate.Year)
                {
                    months = endDate.Month - startDate.Month;
                }
                else
                {
                    months = (12 - startDate.Month) + endDate.Month;
                }

                // Check if last month was a complete month.
                if (endDate < startDate.AddMonths(months) && months != 0)
                {
                    months--;
                }

                // Calculate the number of days.
                startDate = startDate.AddMonths(months);

                days = (endDate - startDate).Days;

                return new Age(years, months, days);
            }
        }


    }
}
