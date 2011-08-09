namespace Microsoft.Samples.SocialGames.Entities
{
    using System;
    using System.Collections.Generic;

    public class GameQueue
    {
        private DateTime creationTime;

        public GameQueue()
        {
            this.CreationTime = DateTime.UtcNow;
            this.Users = new List<GameUser>();
        }

        public Guid Id { get; set; }

        public QueueStatus Status { get; set; }

        public Guid GameId { get; set; }

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

        public List<GameUser> Users { get; set; }
    }
}