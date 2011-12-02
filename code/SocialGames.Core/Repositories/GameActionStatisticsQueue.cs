namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure;

    public class GameActionStatisticsQueue : IGameActionStatisticsQueue
    {
        private readonly IAzureQueue<GameActionMessage> gameActionStatisticGameQueue;

        public GameActionStatisticsQueue()
            : this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"))
        { 
        }

        public GameActionStatisticsQueue(CloudStorageAccount account)
            : this(new AzureQueue<GameActionMessage>(account, ConfigurationConstants.GameActionStatisticsQueue))
        { 
        }

        public GameActionStatisticsQueue(IAzureQueue<GameActionMessage> gameActionStatisticsQueue)
        {
            if (gameActionStatisticsQueue == null)
            {
                throw new ArgumentNullException("gameActionStatisticsQueue");
            }

            this.gameActionStatisticGameQueue = gameActionStatisticsQueue;
            this.gameActionStatisticGameQueue.EnsureExist();
        }

        public void Add(GameAction gameAction)
        {
            if (gameAction == null)
            {
                throw new ArgumentException("gameAction");
            }

            GameActionMessage message = new GameActionMessage() { GameAction = gameAction };

            this.gameActionStatisticGameQueue.AddMessage(message);
        }
   }
}