namespace Microsoft.Samples.SocialGames.Entities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Helpers;

    public class Game
    {
        private DateTime creationTime;

        public Game()
        {
            // Force an UTC date
            this.CreationTime = DateTime.UtcNow;
            this.GameActions = new List<GameAction>();
            this.Users = new List<GameUser>();
        }

        public Guid Id { get; set; }

        public DateTime CreationTime
        {
            get
            {
                return this.creationTime;
            }

            set
            {
                this.creationTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        public int Seed { get; set; }

        public GameStatus Status { get; set; }

        public List<GameUser> Users { get; set; }

        public string ActiveUser { get; set; }

        public List<GameAction> GameActions { get; set; }

        public override bool Equals(object obj)
        {
            return (obj is Game) && this.Equals((Game)obj);
        }

        public bool Equals(Game game)
        {
            bool result = this.Id == game.Id &&
                   this.CreationTime == game.CreationTime &&
                   new CollectionHaveSameElementsComparison<GameUser>(this.Users, game.Users).DoIt() &&
                   this.Status == game.Status;
            
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}