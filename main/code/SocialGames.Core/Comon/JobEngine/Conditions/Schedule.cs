namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System;
    using System.Threading;

    public static class Schedule
    {
        public static Func<bool> Every(TimeSpan interval)
        {
            return new ScheduleCondition(interval).TickFunc;
        }

        public static Func<bool> Every(int milliseconds)
        {
            return new ScheduleCondition(TimeSpan.FromMilliseconds(milliseconds)).TickFunc;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Called from long running processes")]
        private class ScheduleCondition
        {
            private TimeSpan interval;
            private System.Timers.Timer timer;
            private AutoResetEvent signal = new AutoResetEvent(false);

            public ScheduleCondition(TimeSpan interval)
            {
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
        }
    } 
}