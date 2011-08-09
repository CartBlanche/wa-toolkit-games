namespace Microsoft.Samples.SocialGames.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FixedTimeProvider : ITimeProvider
    {
        public DateTime CurrentDateTime { get; set; }
    }
}