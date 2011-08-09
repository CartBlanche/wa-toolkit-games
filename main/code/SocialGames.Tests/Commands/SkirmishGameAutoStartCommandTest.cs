namespace Microsoft.Samples.SocialGames.Worker.Tests.Commands
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common;
    using Microsoft.Samples.SocialGames.Common.JobEngine;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.GamePlay;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Commands;
    using Microsoft.Samples.SocialGames.Worker.Commands;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class SkirmishGameAutoStartCommandTest : CommandTest
    {
        private FixedTimeProvider timeProvider;

        [TestInitialize]
        public void MyTestInitialize()
        {
            this.timeProvider = new FixedTimeProvider();
            TimeProvider.Current = this.timeProvider;
        }

        [TestMethod]
        public void ShouldConstructorUseDefaultValuesWhenNotSupplied()
        {
            // Arrange: We create the mock dependencies
            var gameRepository = new Mock<IGameRepository>();
            var workerContext = new Mock<IWorkerContext>();

            // Act: We create new command without suppling values
            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object);

            // Assert: We verify the values of the not supplied fields are the default values
            Assert.AreEqual(this.GetPrivateFieldValue(command, "gameQueueTimeoutWaiting"), ConfigurationConstants.WaitingForPlayersTimeout);
            Assert.AreEqual(this.GetPrivateFieldValue(command, "maxNumberOfPlayersPerGame"), ConfigurationConstants.MaxNumberOfPlayersPerGame);
        }

        [TestMethod]
        public void ShouldCurrentGameQueueIdBeEmptyByDefault()
        {
            // Arrange: We create the mock dependencies and the new command
            var gameRepository = new Mock<IGameRepository>();
            var workerContext = this.CreateMockWorkerContext();
            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object);

            // Act: We ask for the CurrentGameId
            var currentGameQueueId = command.CurrentGameQueueId;

            // Assert: We verify the CurrentGameId is empty
            Assert.AreEqual(Guid.Empty, currentGameQueueId);
        }

        [TestMethod]
        public void ShouldCurrentGameQueueIdReturnSetValue()
        {
            // Arrange: We create the mock dependencies and the new command. We set the game ID.
            //          We initialize currentGame to any value
            var gameRepository = new Mock<IGameRepository>();
            var workerContext = this.CreateMockWorkerContext();
            workerContext.Object.Context.TryAdd("currentGameQueueId", Guid.NewGuid());
            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object);
            var currentGameQueueId = Guid.NewGuid();

            // Act: We set the game ID, and then we retrieve it
            command.CurrentGameQueueId = currentGameQueueId;
            var returnedGameID = command.CurrentGameQueueId;

            // Assert: We verify the returnedGameID is the game ID we set
            Assert.AreEqual(currentGameQueueId, returnedGameID);
        }

        [TestMethod]
        public void ShouldDoDoNothingIfGameQueueIdIsEmpty()
        {
            // Arrange: Create current game with empty ID
            var gameQueue = new GameQueue
            {
                Id = Guid.Empty,
                Users = new List<GameUser> { new GameUser { UserId = "johnny" } },
                Status = QueueStatus.Waiting,
                CreationTime = DateTime.Parse("01/01/2011 13:00:00")
            };

            int usersCount = gameQueue.Users.Count();

            // Arrange: Create mock dependencies
            //          Set current game in repository and workerContext
            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGameQueue(gameQueue.Id)).Returns(gameQueue);
            gameRepository.Setup(m => m.AddOrUpdateGameQueue(gameQueue));
            var workerContext = this.CreateMockWorkerContext();
            workerContext.Object.Context.TryAdd("currentGameQueueId", gameQueue.Id);

            // Arrange: Create the command
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            int maxNumberOfPlayers = 3;
            var command = new GameQueueAutoStartCommand(
                gameRepository.Object, 
                workerContext.Object, 
                timeout, 
                maxNumberOfPlayers);

            // Arrange: Set current date time to timeout the game
            TimeProvider.Current = new FixedTimeProvider()
            {
                CurrentDateTime = gameQueue.CreationTime + timeout
            };

            // Act: Execute the command. It should do nothing as the game ID is empty
            command.Do(null);

            // Assert: Verify that the game wasn't modified
            Assert.AreEqual(gameQueue.Status, QueueStatus.Waiting);
            Assert.AreEqual(gameQueue.Users.Count(), usersCount);
        }

        [TestMethod]
        [Ignore]
        public void ShouldDoAddBotsWhenNotEnoughtPlayers()
        {
            // Arrange: Create current game and Mock Dependencies
            var johnnyId = "johnny";
            var peterId = "Peter";
            var gameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = johnnyId }, new GameUser { UserId = peterId } },
                Status = QueueStatus.Waiting,
                CreationTime = DateTime.Parse("01/01/2011 13:00:00")
            };

            var gameRepository = this.CreateGameRepository(gameQueue);
            var workerContext = this.CreateMockWorkerContext(gameQueue);
            var humanPlayersCount = gameQueue.Users.Count();

            // Arrange: Create the command
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            int maxNumberOfPlayers = 3;
            var command = new GameQueueAutoStartCommand(
                gameRepository.Object,
                workerContext.Object,
                timeout, 
                maxNumberOfPlayers);

            // Arrange: Set current date time to timeout the game
            this.SetCurrentDateTime(gameQueue.CreationTime + timeout);

            // Act: Execute the command
            command.Do(null);

            // Assert: Verify that the game was modified to start with bots
            Assert.AreEqual(gameQueue.Status, QueueStatus.Ready);
            Assert.AreEqual(gameQueue.Users.Count(), maxNumberOfPlayers);
            Assert.IsTrue(gameQueue.Users.Any(u => u.UserId == johnnyId));
            Assert.AreEqual(this.BotsCount(gameQueue), maxNumberOfPlayers - humanPlayersCount);
        }

        [TestMethod]
        [Ignore]
        public void ShouldDoNotRemovePlayersWhenAddingBots()
        {
            // Arrange: Create current game and Mock Dependencies
            var johnnyId = "johnny";
            var peterId = "peter";
            var gameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = johnnyId }, new GameUser { UserId = peterId } },
                Status = QueueStatus.Waiting,
                CreationTime = DateTime.Parse("01/01/2011 13:00:00")
            };

            var gameRepository = this.CreateGameRepository(gameQueue);
            var workerContext = this.CreateMockWorkerContext(gameQueue);
            var humanPlayersCount = gameQueue.Users.Count();

            // Arrange: Create the command
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            int maxNumberOfPlayers = 3;
            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object, timeout, maxNumberOfPlayers);

            // Arrange: Set current date time to timeout the game
            this.SetCurrentDateTime(gameQueue.CreationTime + timeout);

            // Act: Execute the command
            command.Do(null);

            // Assert: Verify that the game was modified to start with bots but still has johnny and peter
            Assert.AreEqual(gameQueue.Status, QueueStatus.Ready);
            Assert.AreEqual(gameQueue.Users.Count(), maxNumberOfPlayers);
            Assert.IsTrue(gameQueue.Users.Any(u => u.UserId == johnnyId));
            Assert.IsTrue(gameQueue.Users.Any(u => u.UserId == peterId));
            Assert.AreEqual(this.BotsCount(gameQueue), maxNumberOfPlayers - humanPlayersCount);
        }

        [TestMethod]
        public void ShouldDoNotAutoStartIfEnoughtPlayers()
        {
            // Arrange: Create current game and Mock Dependencies
            var johnnyId = "johnny";
            var peterId = "peter";
            var lanaId = "lana";
            var gameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = johnnyId }, new GameUser { UserId = peterId }, new GameUser { UserId = lanaId } },
                Status = QueueStatus.Waiting,
                CreationTime = DateTime.Parse("01/01/2011 13:00:00")
            };

            var gameRepository = this.CreateGameRepository(gameQueue);
            var workerContext = this.CreateMockWorkerContext(gameQueue);
            var humanPlayersCount = gameQueue.Users.Count();

            // Arrange: Create the command
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            int maxNumberOfPlayers = 3;
            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object, timeout, maxNumberOfPlayers);

            // Arrange: Set current date time to timeout the game
            this.SetCurrentDateTime(gameQueue.CreationTime + timeout);

            // Act: Execute the command
            command.Do(null);

            // Assert: Verify that the game was modified to start with bots but still has johnny and peter
            Assert.AreEqual(gameQueue.Status, QueueStatus.Waiting);
            Assert.AreEqual(gameQueue.Users.Count(), maxNumberOfPlayers);
            Assert.IsTrue(gameQueue.Users.Any(u => u.UserId == johnnyId));
            Assert.IsTrue(gameQueue.Users.Any(u => u.UserId == peterId));
            Assert.IsTrue(gameQueue.Users.Any(u => u.UserId == lanaId));
            Assert.AreEqual(this.BotsCount(gameQueue), 0);
        }

        [TestMethod]
        public void ShouldDoNotStartSameGameTwiceIfWeExecuteTwoCommands()
        {
            // Arrange: Create current game and Mock Dependencies
            var johnnyId = "johnny";
            var peterId = "peter";

            var gameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = johnnyId }, new GameUser { UserId = peterId } },
                Status = QueueStatus.Waiting,
                CreationTime = DateTime.Parse("01/01/2011 13:00:00")
            };

            var gameRepository = this.CreateGameRepository(gameQueue);
            var workerContext = this.CreateMockWorkerContext(gameQueue);

            // Arrange: Create the command
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            int maxNumberOfPlayers = 3;
            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object, timeout, maxNumberOfPlayers);

            // Arrange: Set current date time to timeout the game
            this.SetCurrentDateTime(gameQueue.CreationTime + timeout);

            // Arrange: We execute the command for the first time
            command.Do(null);

            // Arrange: We simulate some play has been done, and peter decided to quit
            gameQueue.Users.RemoveAll(u => u.UserId == peterId);
            Assert.AreEqual(gameQueue.Users.Count(), 1);

            // Act: The command shouldn't find any current game, and should do nothing
            command.Do(null);

            // Assert: We verify that our game still has only 1 player (as peter quit)
            Assert.AreEqual(gameQueue.Status, QueueStatus.Ready);
            Assert.AreEqual(gameQueue.Users.Count(), 1);
            Assert.IsTrue(gameQueue.Users.Any(u => u.UserId == johnnyId));
            Assert.IsFalse(gameQueue.Users.Any(u => u.UserId == peterId));
            Assert.AreEqual(this.BotsCount(gameQueue), 0);
        }

        [TestMethod]
        public void ShouldDoDoNothingIfGameHasntTimedOut()
        {
            // Arrange: Create current game
            var gameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = "johnny" } },
                Status = QueueStatus.Waiting,
                CreationTime = DateTime.Parse("01/01/2011 13:00:00")
            };

            int usersCount = gameQueue.Users.Count();

            // Arrange: Create mock dependencies
            //          Set current game in repository and workerContext
            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGameQueue(gameQueue.Id)).Returns(gameQueue);
            gameRepository.Setup(m => m.AddOrUpdateGameQueue(gameQueue));
            var workerContext = this.CreateMockWorkerContext();
            workerContext.Object.Context.TryAdd("currentGameQueueId", gameQueue.Id);

            // Arrange: Create the command
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            int maxNumberOfPlayers = 3;
            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object, timeout, maxNumberOfPlayers);

            // Arrange: Set current date time so the game hasn't timed out yet
            TimeProvider.Current = new FixedTimeProvider()
            {
                CurrentDateTime = gameQueue.CreationTime + timeout - TimeSpan.FromSeconds(1)
            };

            // Act: Execute the command. It should do nothing as the game hasn't timed out yet
            command.Do(null);

            // Assert: Verify that the game wasn't modified
            Assert.AreEqual(gameQueue.Status, QueueStatus.Waiting);
            Assert.AreEqual(gameQueue.Users.Count(), usersCount);
        }

        [TestMethod]
        public void ShouldDoDoNothingIfCurrentGameDoesntExistInWorkingContext()
        {
            // Arrange: Create current game
            var gameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = "johnny" } },
                Status = QueueStatus.Waiting,
                CreationTime = DateTime.Parse("01/01/2011 13:00:00")
            };

            int usersCount = gameQueue.Users.Count();

            // Arrange: Create mock dependencies
            //          Set current game in repository
            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGameQueue(gameQueue.Id)).Returns(gameQueue);
            gameRepository.Setup(m => m.AddOrUpdateGameQueue(gameQueue));
            var workerContext = this.CreateMockWorkerContext();

            // Arrange: Intentionally we are NOT doing the next step
            // workerContext.Object.Context.TryAdd("currentGameQueueId", game.Id);

            // Arrange: Create the command
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            int maxNumberOfPlayers = 3;
            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object, timeout, maxNumberOfPlayers);

            // Arrange: Set current date time so the game has timed out
            TimeProvider.Current = new FixedTimeProvider()
            {
                CurrentDateTime = gameQueue.CreationTime + timeout
            };

            // Act: Execute the command. It should do nothing as the current game isn't in the worker context
            command.Do(null);

            // Assert: Verify that the game wasn't modified
            Assert.AreEqual(gameQueue.Status, QueueStatus.Waiting);
            Assert.AreEqual(gameQueue.Users.Count(), usersCount);
        }

        [TestMethod]
        public void ShouldDoDoNothingIfCurrentGameDoesntHaveAnyUser()
        {
            // Arrange: Create current game
            var gameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { },
                Status = QueueStatus.Waiting,
                CreationTime = DateTime.Parse("01/01/2011 13:00:00")
            };

            int usersCount = gameQueue.Users.Count();

            // Arrange: Create mock dependencies
            //          Set current game in repository
            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGameQueue(gameQueue.Id)).Returns(gameQueue);
            gameRepository.Setup(m => m.AddOrUpdateGameQueue(gameQueue));
            var workerContext = this.CreateMockWorkerContext();

            // Arrange: Intentionally we are NOT doing the next step
            workerContext.Object.Context.TryAdd("currentGameQueueId", gameQueue.Id);

            // Arrange: Create the command
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            int maxNumberOfPlayers = 3;
            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object, timeout, maxNumberOfPlayers);

            // Arrange: Set current date time so the game has timed out
            TimeProvider.Current = new FixedTimeProvider()
            {
                CurrentDateTime = gameQueue.CreationTime + timeout
            };

            // Act: Execute the command. It should do nothing as the current game isn't in the worker context
            command.Do(null);

            // Assert: Verify that the game wasn't modified
            Assert.AreEqual(gameQueue.Status, QueueStatus.Waiting);
            Assert.AreEqual(gameQueue.Users.Count(), usersCount);
        }
        
        [TestMethod]
        [Ignore]
        public void AutoStartSkirmishGameTest()
        {
            var startingDateTime = DateTime.Parse("01/01/2001 00:00:00");
            var testTimeout = TimeSpan.FromSeconds(30);
            var maxNumberOfPlayers = 3;
            var testUser1Id = Guid.NewGuid().ToString();
            var testUser2Id = Guid.NewGuid().ToString();
            var testGameQueue = new GameQueue
            {
                Id = Guid.NewGuid(),
                Users = new List<GameUser> { new GameUser { UserId = testUser1Id }, new GameUser { UserId = testUser2Id } },
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
                          .Callback(() => { timeProvider.CurrentDateTime = startingDateTime + testTimeout; })
                          .Verifiable();

            gameRepository.Setup(m => m.AddOrUpdateGameQueue(It.Is<GameQueue>(g => g.Id == testGameQueue.Id && g.Status == QueueStatus.Ready && g.Users.Count == maxNumberOfPlayers && g.Users.ElementAt(0).UserId == testUser1Id && g.Users.ElementAt(1).UserId == testUser2Id && g.Users.ElementAt(2).UserId.StartsWith("Bot-"))))
                          .Callback(() => { testGameQueue.Id = new Guid(workerContext.Object.Context["currentGameQueueId"].ToString()); })
                          .Verifiable();

            var command = new GameQueueAutoStartCommand(gameRepository.Object, workerContext.Object, testTimeout, maxNumberOfPlayers);
            var context = new Dictionary<string, object>
            {
                { "userId", testUser1Id }
            };

            command.Do(context);

            gameRepository.VerifyAll();
        }

        private object GetPrivateFieldValue<T>(T objectToAccess, string fieldName)
        {
            return objectToAccess.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(objectToAccess);
        }

        private void SetCurrentDateTime(DateTime currentDateTime)
        {
            TimeProvider.Current = new FixedTimeProvider() { CurrentDateTime = currentDateTime };
        }

        private Mock<IWorkerContext> CreateMockWorkerContext(GameQueue gameQueue)
        {
            var workerContext = this.CreateMockWorkerContext();
            workerContext.Object.Context.TryAdd("currentGameQueueId", gameQueue.Id);

            return workerContext;
        }

        private Mock<IGameRepository> CreateGameRepository(GameQueue gameQueue)
        {
            var gameRepository = new Mock<IGameRepository>();
            gameRepository.Setup(m => m.GetGameQueue(gameQueue.Id)).Returns(gameQueue);
            gameRepository.Setup(m => m.AddOrUpdateGameQueue(gameQueue));
            
            return gameRepository;
        }

        private int BotsCount(GameQueue gameQueue)
        {
            return gameQueue.Users.Where(player => player.UserId.StartsWith(ConfigurationConstants.BotUserIdPrefix)).Count();
        }
    }
}