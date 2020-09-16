using System;
using System.Collections.Generic;

namespace BankCurrencyRatesBot.Calendar
{
    public class MyCalendar
    {
        public int CurrentYear { get; set; }
        public int CurrentMonthNumber { get; set; }
        public string CurrentMonthName { get; set; }
        public int CurrentDay { get; set; }
        public string CurrentDayOfWeek { get; set; }
        public Dictionary<string, int> Months = new Dictionary<string, int>()
        {
            { "Jan", 31 }, { "Feb", 29 }, { "Mar", 31 },
            { "Apr", 30 }, { "May", 31 }, { "Jun", 30 },
            { "Jul", 31 }, { "Aug", 31 }, { "Sep", 30 },
            { "Oct", 31 }, { "Nov", 30 }, { "Dec", 31 }
        };
        public Dictionary<string, int> MonthsNumbers = new Dictionary<string, int>()
        {
            { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 },
            { "Apr", 4 }, { "May", 5 }, { "Jun", 6 },
            { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 },
            { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 }
        };

        //public List<string> DaysOfWeek = new List<string>(){ "Mo", "Tu", "We", "Th", "Fr", "Sa", "Su" };
        //private List<string> DaysOfWeek2 = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        public MyCalendar(DateTime date)
        {
            CurrentYear = date.Year;
            CurrentMonthNumber = date.Month;
            CurrentDay = date.Day;

            foreach (var month in MonthsNumbers)
            {
                if (month.Value == CurrentMonthNumber)
                {
                    CurrentMonthName = month.Key;
                    break;
                }
            }

            switch (date.DayOfWeek.ToString())
            {
                case "Monday":
                    CurrentDayOfWeek = ShortDaysOfWeek.Mo.ToString();
                    break;
                case "Tuesday":
                    CurrentDayOfWeek = ShortDaysOfWeek.Tu.ToString();
                    break;
                case "Wednesday":
                    CurrentDayOfWeek = ShortDaysOfWeek.We.ToString();
                    break;
                case "Thursday":
                    CurrentDayOfWeek = ShortDaysOfWeek.Th.ToString();
                    break;
                case "Friday":
                    CurrentDayOfWeek = ShortDaysOfWeek.Fr.ToString();
                    break;
                case "Saturday":
                    CurrentDayOfWeek = ShortDaysOfWeek.Sa.ToString();
                    break;
                case "Sunday":
                    CurrentDayOfWeek = ShortDaysOfWeek.Su.ToString();
                    break;
                default:
                    throw new Exception();
            }
        }


        public void ReplaceEnums(LongDaysOfWeek op)
        {
            switch (op)
            {
                case LongDaysOfWeek.Monday:
                    CurrentDayOfWeek = ShortDaysOfWeek.Mo.ToString();
                    break;
                case LongDaysOfWeek.Tuesday:
                    CurrentDayOfWeek = ShortDaysOfWeek.Tu.ToString();
                    break;
                case LongDaysOfWeek.Wednesday:
                    CurrentDayOfWeek = ShortDaysOfWeek.We.ToString();
                    break;
                case LongDaysOfWeek.Thursday:
                    CurrentDayOfWeek = ShortDaysOfWeek.Th.ToString();
                    break;
                case LongDaysOfWeek.Friday:
                    CurrentDayOfWeek = ShortDaysOfWeek.Fr.ToString();
                    break;
                case LongDaysOfWeek.Saturday:
                    CurrentDayOfWeek = ShortDaysOfWeek.Sa.ToString();
                    break;
                case LongDaysOfWeek.Sunday:
                    CurrentDayOfWeek = ShortDaysOfWeek.Su.ToString();
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}