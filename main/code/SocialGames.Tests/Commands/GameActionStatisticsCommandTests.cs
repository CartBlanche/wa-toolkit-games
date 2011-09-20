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
        public void SendPlayerDeadCommand()
        {
            string userId = Guid.NewGuid().ToString();

            GameAction gameAction = new GameAction()
            {
                UserId = userId,
                Type = 1,
                Id = Guid.NewGuid(),
                CommandData = new Dictionary<string, object>()
                { 
                    { "accuracy", 1 },
                    { "kills", 1 },
                    { "terrainDeformation", 1 },
                    { "xp", 1 },
                }
            };

            this.command.Do(gameAction);

            var result = this.repository.Retrieve(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Accuracy);
            Assert.AreEqual(1, result.TerrainDeformation);
            Assert.AreEqual(1, result.Kills);
            Assert.AreEqual(1, result.XP);            
        }

        [TestMethod]
        public void SendEndGameCommand()
        {
            string userId = Guid.NewGuid().ToString();

            GameAction gameAction = new GameAction()
            {
                UserId = userId,
                Type = GameActionType.EndGame,
                Id = Guid.NewGuid(),
                CommandData = new Dictionary<string, object>()
                { 
                    { "xp", 1 },            
                }
            };

            this.command.Do(gameAction);

            var result = this.repository.Retrieve(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Victories);
            Assert.AreEqual(1, result.XP);
        }

        [TestMethod]
        public void CalculateAverage()
        {
            string userId = Guid.NewGuid().ToString();

            this.repository.Save(new UserStats
            {
                UserId = userId,
                Kills = 100,
            });

            GameAction gameAction = new GameAction()
            {
                UserId = userId,
                Type = GameActionType.EndGame,
                Id = Guid.NewGuid(),
                CommandData = new Dictionary<string, object>()
                { 
                    { "kills", 200 },            
                }
            };

            this.command.Do(gameAction);

            var result = this.repository.Retrieve(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(150, result.Kills);            
        }
    }
}
