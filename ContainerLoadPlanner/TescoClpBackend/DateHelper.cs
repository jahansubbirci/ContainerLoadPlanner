using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TescoClpBackend
{
    internal class DateHelper
    {

        public static DateTime GetClosestPastEHD(DateTime date, DayOfWeek ehd)
        {
            // Find the day of the week for the given date
            DayOfWeek currentDay = date.DayOfWeek;

            // Calculate the number of days to add or subtract to get to Monday
            //int daysToAdd = ((int)DayOfWeek.Monday - (int)currentDay + 7) % 7;
            int daysToSubtract = ((int)currentDay - (int)ehd + 7) % 7;

            // Get the previous and next Monday
            //DateTime nextMonday = date.AddDays(daysToAdd == 0 ? 7 : daysToAdd); // Avoid returning the same day if it is Monday
            DateTime previousMonday = date.AddDays(daysToSubtract == 0 ? -7 : -daysToSubtract); // Avoid returning the same day if it is Monday

            // Determine which Monday is closer
            //TimeSpan toNextMonday = nextMonday - date;
            TimeSpan toPreviousMonday = date - previousMonday;

            //if (toNextMonday <= toPreviousMonday)
            //{
            //    return nextMonday;
            //}
            //else
            //{
            return previousMonday;
            //}
        }
    }
}

