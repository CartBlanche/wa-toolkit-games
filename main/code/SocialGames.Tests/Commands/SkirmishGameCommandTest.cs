namespace Microsoft.Samples.SocialGames.Worker.Tests.Commands
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.SocialGames.Common;
    using Microsoft.Samples.SocialGames.Common.JobEngine;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Commands;
    using Microsoft.Samples.SocialGames.Worker.Commands;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class SkirmishGameCommandTest : CommandTest
    {
        private FixedTimeProvider timeProvider;

        [TestInitialize]
        public void MyTestInitialize()
        {
            this.timeProvider = new FixedTimeProvider();
            TimeProvider.Current = this.timeProvider;
        }

        [TestMethod]
        public void AddUserToNewSkirmishGameTest()
        {
            var testUserId = Guid.NewGuid().ToString();
            var gameId = Guid.Empty;

            var workerContext = new Mock<IWorkerContext>();
            workerContext.SetupGet(p => p.Context).Returns(new ConcurrentDictionary<string, object>());

            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGameQueue(It.Is<Guid>(g => g == Guid.Empty)))
                          .Verifiable();

            gameRepository.Setup(m => m.AddOrUpdateGameQueue(It.Is<GameQueue>(g => g.Id != Guid.Empty && g.Status == QueueStatus.Waiting && g.Users.Count == 1 && g.Users.First().UserId == testUserId)))
                          .Callback(() => { gameId = new Guid(workerContext.Object.Context["currentGameQueueId"].ToString()); })
                          .Verifiable();

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(m => m.GetUser(It.Is<string>(u => u == testUserId)))
                          .Returns(new UserProfile { Id = testUserId })
                          .Verifiable();
            userRepository.Setup(m => m.AddOrUpdateUserSession(It.Is<UserSession>(s => s.UserId == testUserId && s.ActiveGameQueueId == gameId)))
                          .Verifiable();

            var command = new SkirmishGameQueueCommand(userRepository.Object, gameRepository.Object, workerContext.Object);
            var context = new Dictionary<string, object>
            {
                { "userId", testUserId }
            };

            command.Do(context);

            gameRepository.VerifyAll();
            userRepository.VerifyAll();
        }

        [TestMethod]
        public void ShouldCurrentGameIdBeEmptyIfNotInContext()
        {
            // Arrange: We create the mock dependencies and the new command
            //          We make sure the currentGame doesn't exist in the context
            var userRepository = new Mock<IUserRepository>();
            var gameRepository = new Mock<IGameRepository>();
            var workerContext = CreateMockWorkerContext();
            var command = new SkirmishGameQueueCommand(userRepository.Object, gameRepository.Object, workerContext.Object);
            object value;
            workerContext.Object.Context.TryRemove("currentGameQueueId", out value);

            // Act: We ask for the CurrentGameId
            var currentGameId = command.CurrentGameQueueId;

            // Assert: We verify the CurrentGameId is empty
            Assert.AreEqual(Guid.Empty, currentGameId);
        }

        [TestMethod]
        public void AddUserToNewSkirmishGameWhenCurrentGameIsReadyTest()
        {
            var startingDateTime = DateTime.Parse("01/01/2001 00:00:00");
            var testUserId = Guid.NewGuid().ToString();
            var testGameId2 = Guid.Empty;
            var testGameQueue1 = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = testUserId } },
                Status = QueueStatus.Ready,
                CreationTime = startingDateTime
            };

            var workerContext = new Mock<IWorkerContext>();
            var testContext = new ConcurrentDictionary<string, object>();
            testContext.GetOrAdd("currentGameQueueId", testGameQueue1.Id);
            workerContext.SetupGet(p => p.Context).Returns(testContext);

            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGameQueue(It.Is<Guid>(g => g == testGameQueue1.Id)))
                          .Returns(testGameQueue1)
                          .Verifiable();

            gameRepository.Setup(m => m.AddOrUpdateGameQueue(It.Is<GameQueue>(g => g.Id != Guid.Empty && g.Id != testGameQueue1.Id && g.Status == QueueStatus.Waiting && g.Users.Count == 1 && g.Users.First().UserId == testUserId)))
                          .Callback(() => { testGameId2 = new Guid(workerContext.Object.Context["currentGameQueueId"].ToString()); })
                          .Verifiable();

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(m => m.GetUser(It.Is<string>(u => u == testUserId)))
                          .Returns(new UserProfile { Id = testUserId })
                          .Verifiable();
            userRepository.Setup(m => m.AddOrUpdateUserSession(It.Is<UserSession>(s => s.UserId == testUserId && s.ActiveGameQueueId == testGameId2)))
                          .Verifiable();

            var command = new SkirmishGameQueueCommand(userRepository.Object, gameRepository.Object, workerContext.Object);
            var context = new Dictionary<string, object>
            {
                { "userId", testUserId }
            };

            command.Do(context);

            gameRepository.VerifyAll();
            userRepository.VerifyAll();
        }

        [TestMethod]
        public void AddUserToAnExistingSkirmishGameTest()
        {
            var startingDateTime = DateTime.Parse("01/01/2001 00:00:00");
            var testUserId1 = Guid.NewGuid().ToString();
            var testUserId2 = Guid.NewGuid().ToString();
            var testGameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = testUserId1 } },
                Status = QueueStatus.Waiting,
                CreationTime = startingDateTime
            };

            var workerContext = new Mock<IWorkerContext>();
            var testContext = new ConcurrentDictionary<string, object>();
            testContext.GetOrAdd("currentGameQueueId", testGameQueue.Id);
            workerContext.SetupGet(p => p.Context).Returns(testContext);

            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGameQueue(It.Is<Guid>(g => g == testGameQueue.Id)))
                          .Returns(testGameQueue)
                          .Verifiable();

            gameRepository.Setup(m => m.AddOrUpdateGameQueue(It.Is<GameQueue>(g => g.Id == testGameQueue.Id && g.Status == QueueStatus.Waiting && g.Users.Count == 2 && g.Users.First().UserId == testUserId1 && g.Users.Last().UserId == testUserId2)))
                          .Verifiable();

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(m => m.GetUser(It.Is<string>(u => u == testUserId2)))
                          .Returns(new UserProfile { Id = testUserId2 })
                          .Verifiable();
            userRepository.Setup(m => m.AddOrUpdateUserSession(It.Is<UserSession>(s => s.UserId == testUserId2 && s.ActiveGameQueueId == testGameQueue.Id)))
                          .Verifiable();

            var command = new SkirmishGameQueueCommand(userRepository.Object, gameRepository.Object, workerContext.Object);
            var context = new Dictionary<string, object>
            {
                { "userId", testUserId2 }
            };

            command.Do(context);

            gameRepository.VerifyAll();
            userRepository.VerifyAll();
        }

        [TestMethod]
        public void AddUsersToCompleteAnExistingSkirmishGameTest()
        {
            var maxNumberOfPlayersPerGame = 3;
            var startingDateTime = DateTime.Parse("01/01/2001 00:00:00");
            var testUserId1 = Guid.NewGuid().ToString();
            var testUserId2 = Guid.NewGuid().ToString();
            var testUserId3 = Guid.NewGuid().ToString();
            var testGameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = testUserId1 } },
                Status = QueueStatus.Waiting,
                CreationTime = startingDateTime
            };

            var workerContext = new Mock<IWorkerContext>();
            var testContext = new ConcurrentDictionary<string, object>();
            testContext.GetOrAdd("currentGameQueueId", testGameQueue.Id);
            workerContext.SetupGet(p => p.Context).Returns(testContext);

            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGameQueue(It.Is<Guid>(g => g == testGameQueue.Id)))
                          .Returns(testGameQueue)
                          .Verifiable();

            gameRepository.Setup(m => m.AddOrUpdateGameQueue(It.Is<GameQueue>(g => g.Id == testGameQueue.Id && g.Status == QueueStatus.Waiting && g.Users.Count == 2 && g.Users.First().UserId == testUserId1 && g.Users.Last().UserId == testUserId2)))
                          .Verifiable();

            gameRepository.Setup(m => m.AddOrUpdateGameQueue(It.Is<GameQueue>(g => g.Id == testGameQueue.Id && g.Status == QueueStatus.Ready && g.Users.Count == 3 && g.Users.ElementAt(0).UserId == testUserId1 && g.Users.ElementAt(1).UserId == testUserId2 && g.Users.ElementAt(2).UserId == testUserId3)))
                          .Verifiable();

            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(m => m.GetUser(It.Is<string>(u => u == testUserId2)))
                          .Returns(new UserProfile { Id = testUserId2 })
                          .Verifiable();
            userRepository.Setup(m => m.GetUser(It.Is<string>(u => u == testUserId3)))
                          .Returns(new UserProfile { Id = testUserId3 })
                          .Verifiable();
            userRepository.Setup(m => m.AddOrUpdateUserSession(It.Is<UserSession>(s => s.UserId == testUserId2 && s.ActiveGameQueueId == testGameQueue.Id)))
                          .Verifiable();

            var command = new SkirmishGameQueueCommand(userRepository.Object, gameRepository.Object, workerContext.Object, maxNumberOfPlayersPerGame, TimeSpan.FromSeconds(60));
            var context = new Dictionary<string, object>
            {
                { "userId", testUserId2 }
            };

            command.Do(context);

            context["userId"] = testUserId3;
            command.Do(context);

            gameRepository.VerifyAll();
            userRepository.VerifyAll();
        }
    }
}