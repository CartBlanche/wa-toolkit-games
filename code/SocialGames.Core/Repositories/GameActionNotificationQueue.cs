namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure;

    public class GameActionNotificationQueue : IGameActionNotificationQueue
    {
        private readonly IAzureQueue<GameActionMessage> gameActionNotificationGameQueue;

        public GameActionNotificationQueue()
            : this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"))
        { 
        }

        public GameActionNotificationQueue(CloudStorageAccount account)
            : this(new AzureQueue<GameActionMessage>(account, ConfigurationConstants.GameActionNotificationsQueue))
        { 
        }

        public GameActionNotificationQueue(IAzureQueue<GameActionMessage> gameActionNotificationQueue)
        {
            if (gameActionNotificationQueue == null)
            {
                throw new ArgumentNullException("gameActionNotificationQueue");
            }

            this.gameActionNotificationGameQueue = gameActionNotificationQueue;
            this.gameActionNotificationGameQueue.EnsureExist();
        }

        public void Add(GameAction gameAction)
        {
            if (gameAction == null)
            {
                throw new ArgumentException("gameAction");
            }

            GameActionMessage message = new GameActionMessage() { GameAction = gameAction };

            this.gameActionNotificationGameQueue.AddMessage(message);
        }
   }
}