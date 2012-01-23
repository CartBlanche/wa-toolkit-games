namespace Microsoft.Samples.SocialGames.Common
{
    using System;

    public class TimeProvider : ITimeProvider
    {
        private static TimeProvider currentTimeProvider;

        static TimeProvider()
        {
            Current = new UtcTimeProvider();
        }

        public static ITimeProvider Current
        {
            get
            {
                return currentTimeProvider;
            }

            set
            {
                if (value is TimeProvider)
                {
                    currentTimeProvider = (TimeProvider)value;
                }
                else
                {
                    var timeProvider = new TimeProvider();
                    timeProvider.CurrentTimeProviderImplementation = value;
                    currentTimeProvider = timeProvider;
                }
            }
        }

        public ITimeProvider CurrentTimeProviderImplementation { get; set; }

        public DateTime CurrentDateTime
        {
            get
            {
                return this.CurrentTimeProviderImplementation.CurrentDateTime;
            }
        }
    }
}