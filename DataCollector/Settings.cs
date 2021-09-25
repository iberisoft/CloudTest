using System;

namespace DataCollector
{
    class Settings
    {
        public string DbConnection { get; set; }

        public string BrokerHost { get; set; }

        public string BaseTopic { get; set; }

        public SleepModeSettings SleepMode { get; set; }

        public class SleepModeSettings
        {
            public TimeSpan ShortPeriod { get; set; }

            public TimeSpan LongPeriodStart { get; set; }

            public TimeSpan LongPeriodEnd { get; set; }

            public TimeSpan GetRemainingPeriod()
            {
                var time = DateTime.Now.TimeOfDay;
                if (LongPeriodStart < LongPeriodEnd)
                {
                    if (time >= LongPeriodStart && time <= LongPeriodEnd)
                    {
                        return LongPeriodEnd - time;
                    }
                }
                else
                {
                    if (time >= LongPeriodStart)
                    {
                        return TimeSpan.FromDays(1) + LongPeriodEnd - time;
                    }
                    if (time <= LongPeriodEnd)
                    {
                        return LongPeriodEnd - time;
                    }
                }
                return ShortPeriod;
            }
        }
    }
}
