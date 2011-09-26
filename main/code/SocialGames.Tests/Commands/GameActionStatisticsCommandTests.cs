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
        public void SendVictoryCommand()
        {
            string userId = Guid.NewGuid().ToString();

            GameAction gameAction = new GameAction()
            {
                UserId = userId,
                Type = GameActionType.Victory,
                Id = Guid.NewGuid()
            };

            this.command.Do(gameAction);

            var result = this.repository.Retrieve(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Victories);
            Assert.AreEqual(0, result.Defeats);
        }

        [TestMethod]
        public void SendDefeatCommand()
        {
            string userId = Guid.NewGuid().ToString();

            GameAction gameAction = new GameAction()
            {
                UserId = userId,
                Type = GameActionType.Defeat,
                Id = Guid.NewGuid()
            };

            this.command.Do(gameAction);

            var result = this.repository.Retrieve(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Victories);
            Assert.AreEqual(1, result.Defeats);
        }
    }
}
