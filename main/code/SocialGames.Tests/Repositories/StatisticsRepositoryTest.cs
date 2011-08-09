namespace Microsoft.Samples.SocialGames.Tests.Repositories
{
    using System;
    using System.Linq;
    using System.Transactions;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StatisticsRepositoryTest
    {
        // Update if not using SQL Express
        private const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=SocialGames;Integrated Security=True";

        [TestInitialize]
        public void CreateDatabase()
        {
            // Code for creating the database if it does not exists
            var repository = new StatisticsRepository(ConnectionString);
            var result = repository.Retrieve(Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void SaveAndRetrieveUserStatistics()
        {
            using (var ts = new TransactionScope())
            {
                var repository = new StatisticsRepository(ConnectionString);

                var stats = new UserStats()
                {
                    UserId = "testuser",
                    Accuracy = 50,
                    Kills = 1,
                    Rank = 3,
                    TerrainDeformation = 15,
                    Victories = 5,
                };

                repository.Save(stats);

                var result = repository.Retrieve(stats.UserId);

                Assert.AreEqual(result.UserId, stats.UserId);
                Assert.AreEqual(result.Victories, stats.Victories);
                Assert.AreEqual(result.Kills, stats.Kills);
                Assert.AreEqual(result.Rank, stats.Rank);
                Assert.AreEqual(result.TerrainDeformation, stats.TerrainDeformation);
                Assert.AreEqual(result.XP, stats.XP);
            }
        }

        [TestMethod]
        [Ignore]
        public void GenerateLeaderboard()
        {
            using (var ts = new TransactionScope())
            {
                this.BulkInsertTestData();

                var repository = new StatisticsRepository(ConnectionString);
                var count = 5;
                var leaderboard = repository.GenerateLeaderboard(count);

                Assert.AreEqual(6, leaderboard.Count());

                int idBoard = 0;
                foreach (var board in leaderboard)
                {
                    Assert.AreEqual(++idBoard, board.Id);
                    Assert.AreEqual(count, board.Scores.Count());

                    int idScore = 0;
                    foreach (var score in board.Scores)
                    {
                        Assert.AreEqual(++idScore, score.Id);
                    }
                }
            }
        }

        [TestMethod]
        [Ignore]
        public void GenerateLeaderboardFocused()
        {
            using (var ts = new TransactionScope())
            {
                this.BulkInsertTestData();

                var repository = new StatisticsRepository(ConnectionString);
                var count = 2;
                var userFocus = "testuser_50";
                var leaderboard = repository.GenerateLeaderboard(userFocus, count);

                Assert.AreEqual(6, leaderboard.Count());

                int idBoard = 0;
                foreach (var board in leaderboard)
                {
                    Assert.AreEqual(++idBoard, board.Id);
                    Assert.AreEqual((count * 2) + 1, board.Scores.Count());
                }
            }
        }

        [TestMethod]
        public void BulkInsertTestData()
        {
            var repository = new StatisticsRepository(ConnectionString);
            var rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                var stats = new UserStats()
                {
                    UserId = "testuser_" + i.ToString(),
                    Accuracy = rnd.Next(100),
                    Kills = rnd.Next(1000),
                    Rank = rnd.Next(1000),
                    TerrainDeformation = rnd.Next(70),
                    Victories = rnd.Next(1000),
                    XP = rnd.Next(100),
                };

                repository.Save(stats);
            }
        }
    }
}
