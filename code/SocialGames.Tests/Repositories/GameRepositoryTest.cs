namespace Microsoft.Samples.SocialGames.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Samples.SocialGames.Common;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    
    [TestClass]
    public class GameRepositoryTest : RepositoryTest
    {
        private static string skirmishGameMessageQueueName = "skirmishgamequeuetest";
        private static string leaveGameMessageQueueName = "leavegamequeuetest";
        private static string gameContainerName = "gamestest";
        private static string gameQueueContainerName = "gamesqueuestest";
        private static string userContainerName = "userstest";
        private static string inviteQueueName = "invitetest";

        private CloudStorageAccount cloudStorageAccount;
        private AzureQueue<SkirmishGameQueueMessage> skirmishGameMessageQueue;
        private AzureQueue<LeaveGameMessage> leaveGameMessageQueue;
        private AzureBlobContainer<Game> gameContainer;
        private AzureBlobContainer<GameQueue> gameQueueContainer;
        private GameRepository gameRepository;
        private AzureBlobContainer<UserProfile> userContainer;
        private AzureQueue<InviteMessage> inviteQueue;

        [ClassInitialize]
        public static void InitializeWindowsAzureStorageEmulator(TestContext context)
        {
            new EnsureWindowsAzureStorageEmulatorIsRunning().DoIt();
            TimeProvider.Current = new FixedTimeProvider()
                {
                    CurrentDateTime = DateTime.Parse("01/01/2001 00:00:00")
                };
        }

        [TestInitialize]
        public void InitializeGameRepository()
        {
            this.cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            this.skirmishGameMessageQueue = new AzureQueue<SkirmishGameQueueMessage>(this.cloudStorageAccount, skirmishGameMessageQueueName);
            this.leaveGameMessageQueue = new AzureQueue<LeaveGameMessage>(this.cloudStorageAccount, leaveGameMessageQueueName);
            this.gameContainer = new AzureBlobContainer<Game>(this.cloudStorageAccount, gameContainerName, true);
            this.gameQueueContainer = new AzureBlobContainer<GameQueue>(this.cloudStorageAccount, gameQueueContainerName, true);
            this.userContainer = new AzureBlobContainer<UserProfile>(this.cloudStorageAccount, userContainerName, true);
            this.inviteQueue = new AzureQueue<InviteMessage>(this.cloudStorageAccount, inviteQueueName);
            this.gameRepository = new GameRepository(this.gameContainer, this.gameQueueContainer, this.skirmishGameMessageQueue, this.leaveGameMessageQueue, this.userContainer, this.inviteQueue);
            this.gameRepository.Initialize();
            this.skirmishGameMessageQueue.Clear();
            this.leaveGameMessageQueue.Clear();
        }

        [TestMethod]
        public void GetGameReferenceTest()
        {
            Game gameFirstVersion = this.JohnPeterAndBrianGame();
            this.gameRepository.AddOrUpdateGame(gameFirstVersion);
            string address = this.gameRepository.GetGameReference(gameFirstVersion.Id, TimeSpan.FromSeconds(10));
            
            using (var webClient = new WebClient())
            {
                var data = webClient.DownloadString(address);
                var serialized = this.Serialized(gameFirstVersion, true);
                var expected = gameContainerName + serialized;
                
                Assert.AreEqual(expected, data);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.Net.WebException))]
        public void GetGameReferenceTimeOutTest()
        {
            Game gameFirstVersion = this.JohnPeterAndBrianGame();
            this.gameRepository.AddOrUpdateGame(gameFirstVersion);
            TimeSpan timeSpan = TimeSpan.FromSeconds(1);
            string address = this.gameRepository.GetGameReference(gameFirstVersion.Id, timeSpan);
            System.Threading.Thread.Sleep(timeSpan.Add(TimeSpan.FromSeconds(1)));
            System.Net.WebClient webClient = new System.Net.WebClient();
            string data = webClient.DownloadString(address);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddInvalidUserToSkirmishGameQueueTest()
        {
            this.gameRepository.AddUserToGameQueue(string.Empty, GameType.Skirmish);
        }

        [TestMethod]
        public void AddUserToSkirmishGameQueueTest()
        {
            var firstUserID = Guid.NewGuid().ToString();
            var secondUserID = Guid.NewGuid().ToString();
            var thirdUserID = Guid.NewGuid().ToString();
            var fourthUserID = Guid.NewGuid().ToString();

            this.gameRepository.AddUserToGameQueue(firstUserID, GameType.Skirmish);

            Assert.AreEqual(firstUserID, this.skirmishGameMessageQueue.PopMessage().UserId);

            this.gameRepository.AddUserToGameQueue(firstUserID, GameType.Skirmish);
            this.gameRepository.AddUserToGameQueue(secondUserID, GameType.Skirmish);
            this.gameRepository.AddUserToGameQueue(thirdUserID, GameType.Skirmish);
            this.gameRepository.AddUserToGameQueue(fourthUserID, GameType.Skirmish);

            Assert.AreEqual(firstUserID, this.skirmishGameMessageQueue.PopMessage().UserId);
            Assert.AreEqual(secondUserID, this.skirmishGameMessageQueue.PopMessage().UserId);
            Assert.AreEqual(thirdUserID, this.skirmishGameMessageQueue.PopMessage().UserId);
            Assert.AreEqual(fourthUserID, this.skirmishGameMessageQueue.PopMessage().UserId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullGameTest()
        {
            this.gameRepository.AddOrUpdateGame(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddGameWithEmptyIDTest()
        {
            this.gameRepository.AddOrUpdateGame(new Game() { Id = Guid.Empty });
        }

        [TestMethod]
        public void AddGameTest()
        {
            Game firstGame = this.JohnPeterAndBrianGame();
            Game secondGame = this.FrancisGabrielDonAndLukeGame();
            Game thirdGame = this.SarahAndJesiGame();

            this.gameRepository.AddOrUpdateGame(firstGame);
            Assert.AreEqual(firstGame, this.gameRepository.GetGame(firstGame.Id));

            this.gameRepository.AddOrUpdateGame(secondGame);
            Assert.AreEqual(firstGame, this.gameRepository.GetGame(firstGame.Id));
            Assert.AreEqual(secondGame, this.gameRepository.GetGame(secondGame.Id));

            this.gameRepository.AddOrUpdateGame(thirdGame);
            Assert.AreEqual(firstGame, this.gameRepository.GetGame(firstGame.Id));
            Assert.AreEqual(secondGame, this.gameRepository.GetGame(secondGame.Id));
            Assert.AreEqual(thirdGame, this.gameRepository.GetGame(thirdGame.Id));
        }

        [TestMethod]
        public void LeaveUserFromGameTest()
        {
            Game game = this.FrancisGabrielDonAndLukeGame();
            var francisUser = new UserProfile { Id = Guid.NewGuid().ToString(), DisplayName = "Francis" };
            var users = new AzureBlobContainer<UserProfile>(CloudStorageAccount.DevelopmentStorageAccount);
            var sessions = new AzureBlobContainer<UserSession>(CloudStorageAccount.DevelopmentStorageAccount);
            var friends = new AzureBlobContainer<Friends>(CloudStorageAccount.DevelopmentStorageAccount);
            var userRepository = new UserRepository(users, sessions, friends);
            userRepository.AddOrUpdateUser(francisUser);
            this.gameRepository.AddOrUpdateGame(game);
            this.gameRepository.LeaveUserFromGame(francisUser.Id, game.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LeaveUserFromGameWithEmptyIDTest()
        {
            this.gameRepository.LeaveUserFromGame(Guid.NewGuid().ToString(), Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LeaveUserFromGameUserWithEmptyIDTest()
        {
            this.gameRepository.LeaveUserFromGame(string.Empty, Guid.NewGuid());
        }

        [TestMethod]
        public void UpdateGameTest()
        {
            Game firstGame = this.JohnPeterAndBrianGame();
            Game secondGame = this.FrancisGabrielDonAndLukeGame();
            Game thirdGame = this.SarahAndJesiGame();

            this.gameRepository.AddOrUpdateGame(firstGame);
            this.gameRepository.AddOrUpdateGame(secondGame);
            this.gameRepository.AddOrUpdateGame(thirdGame);
            Assert.AreEqual(firstGame, this.gameRepository.GetGame(firstGame.Id));
            Assert.AreEqual(secondGame, this.gameRepository.GetGame(secondGame.Id));
            Assert.AreEqual(thirdGame, this.gameRepository.GetGame(thirdGame.Id));

            Game firstGameModified = this.FrancisGabrielDonAndLukeGame();
            firstGameModified.Id = firstGame.Id;

            this.gameRepository.AddOrUpdateGame(firstGameModified);

            Assert.AreEqual(firstGameModified, this.gameRepository.GetGame(firstGame.Id));
            Assert.AreEqual(secondGame, this.gameRepository.GetGame(secondGame.Id));
            Assert.AreEqual(thirdGame, this.gameRepository.GetGame(thirdGame.Id));

            Game secondGameModified = this.SarahAndJesiGame();
            secondGameModified.Id = secondGame.Id;

            this.gameRepository.AddOrUpdateGame(secondGameModified);

            Assert.AreEqual(firstGameModified, this.gameRepository.GetGame(firstGame.Id));
            Assert.AreEqual(secondGameModified, this.gameRepository.GetGame(secondGame.Id));
            Assert.AreEqual(thirdGame, this.gameRepository.GetGame(thirdGame.Id));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GameRepositoryConstructorWithNullConstructorParameter()
        {
            new GameRepository(null, null, null, null, null, null);
        }

        [TestMethod]
        public void GameRepositoryConstructor()
        {
            IAzureBlobContainer<Game> gameContainer = new AzureBlobContainer<Game>(this.cloudStorageAccount);
            IAzureQueue<SkirmishGameQueueMessage> skirmishGameMessageQueue = new AzureQueue<SkirmishGameQueueMessage>(this.cloudStorageAccount, skirmishGameMessageQueueName);
            IAzureQueue<LeaveGameMessage> leaveGameMessageQueue = new AzureQueue<LeaveGameMessage>(this.cloudStorageAccount, leaveGameMessageQueueName);
            ExceptionAssert.ShouldThrow<ArgumentNullException>(() => new GameRepository(null, null, null, null, null, null));
            ExceptionAssert.ShouldThrow<ArgumentNullException>(() => new GameRepository(gameContainer, null, null, null, null, null));
            ExceptionAssert.ShouldThrow<ArgumentNullException>(() => new GameRepository(null, null, skirmishGameMessageQueue, null, null, null));
            ExceptionAssert.ShouldThrow<ArgumentNullException>(() => new GameRepository(null, null, null, leaveGameMessageQueue, null, null));
            ExceptionAssert.ShouldThrow<ArgumentNullException>(() => new GameRepository(null, null, skirmishGameMessageQueue, leaveGameMessageQueue, null, null));
            ExceptionAssert.ShouldThrow<ArgumentNullException>(() => new GameRepository(gameContainer, null, null, leaveGameMessageQueue, null, null));
            ExceptionAssert.ShouldThrow<ArgumentNullException>(() => new GameRepository(gameContainer, null, skirmishGameMessageQueue, null, null, null));
        }

        [TestMethod]
        public void GameRepositoryDefaultConstructor()
        {
            bool wasSetterCalled = false;
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                if (configName == "DataConnectionString")
                {
                    wasSetterCalled = true;
                    configSetter("UseDevelopmentStorage=true");
                }
            });

            new GameRepository(null, null, null, null, null, null);
            Assert.IsTrue(wasSetterCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGameRepositoryWithNullParameters()
        {
            new GameRepository(null, null, null, null, null, null);
        }

        private Game JohnPeterAndBrianGame()
        {
            return new Game
            {
                Id = Guid.NewGuid(),
                CreationTime = TimeProvider.Current.CurrentDateTime,
                Users = new List<GameUser> { new GameUser { UserId = "john" }, new GameUser { UserId = "peter" }, new GameUser { UserId = "brian" } },
                Status = GameStatus.Waiting,
                Seed = 20
            };
        }

        private Game FrancisGabrielDonAndLukeGame()
        {
            return new Game
            {
                Id = Guid.NewGuid(),
                CreationTime = TimeProvider.Current.CurrentDateTime,
                Users = new List<GameUser> { new GameUser { UserId = "Francis" }, new GameUser { UserId = "Gabriel" }, new GameUser { UserId = "Don" }, new GameUser { UserId = "Luke" } },
                Status = GameStatus.Ready,
                Seed = 20
            };
        }

        private Game SarahAndJesiGame()
        {
            return new Game
            {
                Id = Guid.NewGuid(),
                CreationTime = TimeProvider.Current.CurrentDateTime,
                Users = new List<GameUser> { new GameUser { UserId = "Sarah" }, new GameUser { UserId = "Jesi" } },
                Status = GameStatus.Timeout,
                Seed = 54
            };
        }
    }
}