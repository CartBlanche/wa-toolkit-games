namespace Microsoft.Samples.Tests.GamePlay.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class ConfiguredGameActionProcessor : GameActionProcessor
    {
        private IGameActionNotificationQueue notificationQueue;
        private IGameActionStatisticsQueue statisticsQueue;

        public ConfiguredGameActionProcessor()
            : this(new GameActionNotificationQueue(), new GameActionStatisticsQueue())
        {
        }

        public ConfiguredGameActionProcessor(IGameActionNotificationQueue notificationQueue, IGameActionStatisticsQueue statisticsQueue)
        {
            this.notificationQueue = notificationQueue;
            this.statisticsQueue = statisticsQueue;

            this.Register(this.IsMortalShot, ga => this.SendToNotificationQueue(ga));
            this.Register(GameActionType.PlayerDead, ga => this.SendToStatisticsQueue(ga));
            this.Register(GameActionType.EndGame, ga => this.SendToStatisticsQueue(ga));
        }

        private bool IsMortalShot(GameAction gameAction)
        {
            if (gameAction.Type != GameActionType.Shot)
            {
                return false;
            }

            if (!gameAction.CommandData.ContainsKey("shootedId"))
            {
                return false;
            }

            var shootedId = (string)gameAction.CommandData["shootedId"];

            if (string.IsNullOrEmpty(shootedId) || shootedId == "null")
            {
                return false;
            }

            return true;
        }

        private void SendToNotificationQueue(GameAction gameAction)
        {
            this.notificationQueue.Add(gameAction);
        }

        private void SendToStatisticsQueue(GameAction gameAction)
        {
            this.statisticsQueue.Add(gameAction);
        }
    }
}