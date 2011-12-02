namespace Microsoft.Samples.SocialGames.Entities
{
    using System.Collections.Generic;

    public class Friends
    {
        public Friends()
        {
            this.Users = new List<string>();
        }

        public string Id { get; set; }

        public List<string> Users { get; set; }
    }
}
