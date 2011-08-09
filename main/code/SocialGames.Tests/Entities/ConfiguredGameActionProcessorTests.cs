namespace Microsoft.Samples.SocialGames.Tests.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Mocks;
    using Microsoft.Samples.Tests.GamePlay.Game;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConfiguredGameActionProcessorTests
    {
        [TestMethod]
        public void ShouldQueuePlayerDeadGameCommand()
        {
            MockGameActionStatisticsQueue gameActionStatisticsQueue = new MockGameActionStatisticsQueue();
            ConfiguredGameActionProcessor processor = new ConfiguredGameActionProcessor(new MockGameActionNotificationQueue(), gameActionStatisticsQueue);

            GameAction playerDeadGameAction = new GameAction()
            {
                Type = GameActionType.PlayerDead,
                Id = Guid.NewGuid(),
                UserId = "johnny",
                CommandData = new Dictionary<string, object>() { { "kills", 3 } }
            };

            processor.Process(playerDeadGameAction);
            
            GameAction action = gameActionStatisticsQueue.GetQueuedGameAction();
            Assert.AreEqual(action.Type, GameActionType.PlayerDead);
            Assert.AreEqual((int)action.CommandData["kills"], 3);
        }

        [TestMethod]
        public void ShouldQueueGameEndGameCommand()
        {
            MockGameActionStatisticsQueue gameActionStatisticsQueue = new MockGameActionStatisticsQueue();
            ConfiguredGameActionProcessor processor = new ConfiguredGameActionProcessor(new MockGameActionNotificationQueue(), gameActionStatisticsQueue);

            GameAction playerDeadGameAction = new GameAction()
            {
                Type = GameActionType.EndGame,
                Id = Guid.NewGuid(),
                UserId = "johnny",
                CommandData = new Dictionary<string, object>() { { "kills", 3 } }
            };

            processor.Process(playerDeadGameAction);

            GameAction action = gameActionStatisticsQueue.GetQueuedGameAction();
            Assert.AreEqual(action.Type, GameActionType.EndGame);
            Assert.AreEqual((int)action.CommandData["kills"], 3);
        }  
    }
}
