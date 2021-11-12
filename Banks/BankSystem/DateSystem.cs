using System;

namespace Banks.BankSystem
{
    public class DateSystem
    {
        public DateSystem(DateTime date)
        {
            DateTime = date;
        }

        private DateSystem()
        {
        }

        public delegate void NewDayHandler(DateTime date);
        public delegate void NewMonthHandler(DateTime date);
        public event NewDayHandler NotifyNewDay;
        public event NewMonthHandler NotifyNewMonth;

        public int Id { get; private init; }

        public DateTime DateTime { get; private set; }

        public void SkipDays(int days)
        {
            for (int dayCounter = 1; dayCounter <= days; dayCounter++)
            {
                DateTime newTime = DateTime.AddDays(1);
                NotifyNewDay?.Invoke(newTime);

                if (newTime.Month != DateTime.Month)
                    NotifyNewMonth?.Invoke(newTime);

                DateTime = newTime;
            }
        }
    }
}