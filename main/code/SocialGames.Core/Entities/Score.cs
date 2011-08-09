namespace Microsoft.Samples.SocialGames.Entities
{
    using System;

    public class Score
    {
        public long Id { get; set; }
        
        public string UserId { get; set; }

        public string UserName { get; set; }

        public float XP { get; set; }

        public float Rank { get; set; }

        public float Kills { get; set; }

        public float Victories { get; set; }

        public float Accuracy { get; set; }

        public float TerrainDeformation { get; set; }
    }
}