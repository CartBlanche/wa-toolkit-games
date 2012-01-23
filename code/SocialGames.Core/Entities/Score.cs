namespace Microsoft.Samples.SocialGames.Entities
{

    public class Score
    {
        public long Id { get; set; }
        
        public string UserId { get; set; }

        public string UserName { get; set; }

        public float GameCount { get; set; }

        public float Victories { get; set; }

        public float Defeats { get; set; }
    }
}