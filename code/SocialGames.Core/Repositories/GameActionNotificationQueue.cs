namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure;
    using Microsoft.Samples.SocialGames.Common;

    public class GameActionNotificationQueue : IGameActionNotificationQueue, IInitializer
    {
        private readonly IAzureQueue<GameActionNotificationMessage> gameActionNotificationGameQueue;

        public GameActionNotificationQueue(IAzureQueue<GameActionNotificationMessage> gameActionNotificationQueue)
        {
            if (gameActionNotificationQueue == null)
            {
                throw new ArgumentNullException("gameActionNotificationQueue");
            }

            this.gameActionNotificationGameQueue = gameActionNotificationQueue;
        }

        public void Add(GameAction gameAction)
        {
            if (gameAction == null)
            {
                throw new ArgumentException("gameAction");
            }

            GameActionNotificationMessage message = new GameActionNotificationMessage() { GameAction = gameAction };

            this.gameActionNotificationGameQueue.AddMessage(message);
        }

        public void Initialize()
        {
            this.gameActionNotificationGameQueue.EnsureExist();
        }
    }
}