namespace Microsoft.Samples.SocialGames.Common
{
    using System;

    public class UtcTimeProvider : ITimeProvider
    {
        public DateTime CurrentDateTime
        {
            get { return DateTime.UtcNow; }
        }
    }
}