namespace Microsoft.Samples.SocialGames.Worker.Tests.Commands
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Mocks;
    using Microsoft.Samples.SocialGames.Worker.Commands;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GameActionStatisticsCommandTests
    {
        private GameActionStatisticsCommand command;
        private IStatisticsRepository repository;

        [TestInitialize]
        public void Setup()
        {
            this.repository = new MockStatisticsRepository();
            this.command = new GameActionStatisticsCommand(this.repository);
        }

        [TestMethod]
        public void SendVictoriesMetric()
        {
            string userId = Guid.NewGuid().ToString();
            var commandData = new Dictionary<string, object>();

            commandData.Add("Victories", 1);

            GameAction gameAction = new GameAction()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CommandData = commandData                
            };

            this.command.Do(gameAction);

            var result = this.repository.Retrieve(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Victories);
        }

        [TestMethod]
        public void SendDefeatsMetric()
        {
            string userId = Guid.NewGuid().ToString();
            var commandData = new Dictionary<string, object>();

            commandData.Add("Defeats", 1);

            GameAction gameAction = new GameAction()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CommandData = commandData
            };

            this.command.Do(gameAction);

            var result = this.repository.Retrieve(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Defeats);
        }

        [TestMethod]
        public void SendGameCountMetric()
        {
            string userId = Guid.NewGuid().ToString();
            var commandData = new Dictionary<string, object>();

            commandData.Add("GameCount", 1);

            GameAction gameAction = new GameAction()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CommandData = commandData
            };

            this.command.Do(gameAction);

            var result = this.repository.Retrieve(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.GameCount);
        }
    }
}
