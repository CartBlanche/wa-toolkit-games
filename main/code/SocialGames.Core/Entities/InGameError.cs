namespace Microsoft.Samples.SocialGames.Entities
{
    using System;

    public class InGameError
    {
        public uint Id { get; set; }

        public string UserAgent { get; set; }

        public DateTime Time { get; set; }

        public string Description { get; set; }
    }
}