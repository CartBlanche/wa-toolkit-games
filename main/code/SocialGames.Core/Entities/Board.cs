namespace Microsoft.Samples.SocialGames.Entities
{
    public class Board
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Score[] Scores { get; set; }
    }
}