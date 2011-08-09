namespace Microsoft.Samples.SocialGames.Entities
{
    using System;
    using System.Collections.Generic;

    public class GameAction
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public int Type { get; set; }

        public IDictionary<string, object> CommandData { get; set; }

        public DateTime Timestamp { get; set; }
    }
}