namespace Microsoft.Samples.SocialGames.Entities
{
    using System;
    using System.Collections.Generic;

    public class Weapon
    {
        public Weapon()
        {
            this.Data = new Dictionary<string, object>();
        }

        public Guid Id { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}