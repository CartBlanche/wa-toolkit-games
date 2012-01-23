namespace Microsoft.Samples.SocialGames.Worker.Tests.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Commands;
    using Microsoft.Samples.SocialGames.Worker.Commands;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class LeaveGameCommandTest : CommandTest
    {
        [TestMethod]
        public void LeaveGameTest()
        {
            var testUserId = Guid.NewGuid().ToString();
            var testGame = new Game
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser>
                { 
                    new GameUser { UserId = Guid.NewGuid().ToString() },
                    new GameUser { UserId = testUserId },
                    new GameUser { UserId = Guid.NewGuid().ToString() }
                }
            };

            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGame(It.Is<Guid>(g => g == testGame.Id)))
                          .Returns(testGame)
                          .Verifiable();

            gameRepository.Setup(m => m.AddOrUpdateGame(It.Is<Game>(g => g.Id == testGame.Id && g.Users.Count == 2 && !g.Users.Any(u => u.UserId == testUserId))))
                          .Verifiable();

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(m => m.AddOrUpdateUserSession(It.Is<UserSession>(s => s.UserId == testUserId && s.ActiveGameQueueId == Guid.Empty)))
                          .Verifiable();

            var command = new LeaveGameCommand(userRepository.Object, gameRepository.Object);
            var context = new Dictionary<string, object>
            {
                { "gameId", testGame.Id },
                { "userId", testUserId }
            };

            command.Do(context);

            gameRepository.VerifyAll();
            userRepository.VerifyAll();
        }

        [TestMethod]
        public void LeaveGameQueueTest()
        {
            var testUserId = Guid.NewGuid().ToString();
            var testGameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser>
                { 
                    new GameUser { UserId = Guid.NewGuid().ToString() },
                    new GameUser { UserId = testUserId },
                    new GameUser { UserId = Guid.NewGuid().ToString() }
                }
            };

            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGame(It.Is<Guid>(g => g == testGameQueue.Id)))
                          .Returns((Game)null)
                          .Verifiable();
            gameRepository.Setup(m => m.GetGameQueue(It.Is<Guid>(g => g == testGameQueue.Id)))
                          .Returns(testGameQueue)
                          .Verifiable();

            gameRepository.Setup(m => m.AddOrUpdateGameQueue(It.Is<GameQueue>(g => g.Id == testGameQueue.Id && g.Users.Count == 2 && !g.Users.Any(u => u.UserId == testUserId))))
                          .Verifiable();

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(m => m.AddOrUpdateUserSession(It.Is<UserSession>(s => s.UserId == testUserId && s.ActiveGameQueueId == Guid.Empty)))
                          .Verifiable();

            var command = new LeaveGameCommand(userRepository.Object, gameRepository.Object);
            var context = new Dictionary<string, object>
            {
                { "gameId", testGameQueue.Id },
                { "userId", testUserId }
            };

            command.Do(context);

            gameRepository.VerifyAll();
            userRepository.VerifyAll();
        }

        [TestMethod]
        public void ShouldntLeaveGameFailIfGameDoesntExist()
        {
            // Arrange: We create a mock gameRepository that doesn't have any games (returns null for all get games)
            //          We create the mock user repository
            //          We create the command and the context
            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGame(It.IsAny<Guid>()))
                .Returns((Game)null);
            var userRepository = new Mock<IUserRepository>();
            var command = new LeaveGameCommand(userRepository.Object, gameRepository.Object);
            var context = new Dictionary<string, object>
            {
                { "gameId", Guid.NewGuid() },
                { "userId", "johnny" }
            };

            // Act: We execute the command the game won't exist, but the command shouldn't fail
            command.Do(context);

            // Asssert: No exception should be thrown
        }
    }
}