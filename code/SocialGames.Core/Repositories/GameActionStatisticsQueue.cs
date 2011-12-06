namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure;
    using Microsoft.Samples.SocialGames.Common;

    public class GameActionStatisticsQueue : IGameActionStatisticsQueue, IInitializer
    {
        private readonly IAzureQueue<GameActionStatisticsMessage> gameActionStatisticGameQueue;

        public GameActionStatisticsQueue(IAzureQueue<GameActionStatisticsMessage> gameActionStatisticsQueue)
        {
            if (gameActionStatisticsQueue == null)
            {
                throw new ArgumentNullException("gameActionStatisticsQueue");
            }

            this.gameActionStatisticGameQueue = gameActionStatisticsQueue;
            
        }

        public void Add(GameAction gameAction)
        {
            if (gameAction == null)
            {
                throw new ArgumentException("gameAction");
            }

            GameActionStatisticsMessage message = new GameActionStatisticsMessage() { GameAction = gameAction };

            this.gameActionStatisticGameQueue.AddMessage(message);
        }

        public void Initialize()
        {
            this.gameActionStatisticGameQueue.EnsureExist();
        }
    }
}