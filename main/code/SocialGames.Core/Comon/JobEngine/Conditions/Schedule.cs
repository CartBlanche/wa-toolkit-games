namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System;
    using System.Threading;

    public static class Schedule
    {
        public static Func<bool> Every(TimeSpan interval)
        {
            using (var scheduleCondition = new ScheduleCondition(interval))
            {
                return scheduleCondition.TickFunc;
            }            
        }

        public static Func<bool> Every(int milliseconds)
        {
            using (var scheduleCondition = new ScheduleCondition(TimeSpan.FromMilliseconds(milliseconds)))
            {
                return scheduleCondition.TickFunc;
            }            
        }

        private class ScheduleCondition : IDisposable
        {
            private TimeSpan interval;
            private System.Timers.Timer timer;
            private AutoResetEvent signal;

            public ScheduleCondition(TimeSpan interval)
            {
                this.signal = new AutoResetEvent(false);
                this.interval = interval;
                this.timer = new System.Timers.Timer(this.interval.TotalMilliseconds);
                this.timer.Elapsed += (sender, arg) => { signal.Set(); };
                this.timer.Start();
            }

            public Func<bool> TickFunc
            {
                get
                {
                    return () =>
                    {
                        this.signal.WaitOne();
                        return true;
                    };
                }
            }

            public void Dispose()
            {
                this.signal.Dispose();
                this.timer.Dispose();
            }
        }
    } 
}