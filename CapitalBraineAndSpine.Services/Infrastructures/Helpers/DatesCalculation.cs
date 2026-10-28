namespace CapitalBraineAndSpine.Services.Infrastructures.Helpers
{
    public class DatesCalculation
    {
        public static List<DateTime> GetMonthlyDates(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date must be before or equal to end date.");

            // List to store one date per month
            List<DateTime> MonthlyDates = new List<DateTime>();

            // Initialize the current date to the start date
            DateTime currentDate = startDate;

            // Iterate through months until the current date is before or equal to the end date
            while (currentDate <= endDate)
            {
                // Add the current date to the list
                MonthlyDates.Add(currentDate);

                // Move to the next month
                currentDate = currentDate.AddMonths(1);
            }

            return MonthlyDates;
        }

        public static List<DateTime> GetBiweeklyDates(DateTime startDate, DateTime endDate)
        {
            List<DateTime> biweeklyDates = new List<DateTime>();
            DateTime currentDate = startDate;

            while (currentDate <= endDate)
            {
                biweeklyDates.Add(currentDate);
                currentDate = currentDate.AddDays(14); // Increment by 14 days for biweekly interval
            }

            return biweeklyDates;
        }
        public static List<DateTime> GetWeeklyDates(DateTime startDate, DateTime endDate)
        {
            List<DateTime> weeklyDates = new List<DateTime>();

            DateTime current = startDate;

            while (current <= endDate)
            {
                weeklyDates.Add(current);
                current = current.AddDays(7); // Move to the next week
            }

            return weeklyDates;
        }
        public static List<DateTime> GetDatesWIthInRange(DateTime startDate, DateTime endDate)
        {
            List<DateTime> weeklyDates = new List<DateTime>();

            DateTime current = startDate;

            while (current <= endDate)
            {
                weeklyDates.Add(current);
            }

            return weeklyDates;
        }
    }
}
