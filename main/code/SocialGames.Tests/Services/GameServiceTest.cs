namespace Microsoft.Samples.SocialGames.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Web.Script.Serialization;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.GamePlay.Services;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;

    [TestClass]
    public class GameServiceTest : ServiceTest
    {
        private int suffix;
        private IAzureBlobContainer<Game> gameContainer;
        private IAzureBlobContainer<GameQueue> gameQueueContainer;
        private IAzureBlobContainer<UserProfile> userContainer;
        private IAzureBlobContainer<UserSession> userSessionContainer;
        private IAzureBlobContainer<Friends> friendContainer;
        private IAzureQueue<SkirmishGameQueueMessage> skirmishGameMessageQueue;
        private IAzureQueue<LeaveGameMessage> leaveGameMessageQueue;
        private IAzureQueue<InviteMessage> inviteQueue;

        [TestInitialize]
        public void Setup()
        {
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                string configuration = RoleEnvironment.IsAvailable ?
                    RoleEnvironment.GetConfigurationSettingValue(configName) :
                    ConfigurationManager.AppSettings[configName];

                configSetter(configuration);
            });

            this.suffix = (new Random()).Next(10000);
        }

        [TestCleanup]
        public void Teardown()
        {
            if (this.gameContainer != null)
            {
                this.gameContainer.DeleteContainer();
            }

            if (this.gameQueueContainer != null)
            {
                this.gameQueueContainer.DeleteContainer();
            }

            if (this.userContainer != null)
            {
                this.userContainer.DeleteContainer();
            }

            if (this.userSessionContainer != null)
            {
                this.userSessionContainer.DeleteContainer();
            }

            if (this.friendContainer != null)
            {
                this.friendContainer.DeleteContainer();
            }

            if (this.skirmishGameMessageQueue != null)
            {
                this.skirmishGameMessageQueue.DeleteQueue();
            }

            if (this.leaveGameMessageQueue != null)
            {
                this.leaveGameMessageQueue.DeleteQueue();
            }

            if (this.inviteQueue != null)
            {
                this.inviteQueue.DeleteQueue();
            }
        }

        #region /game/queue/

        [TestMethod]
        public void Queue()
        {
            string userId = "johnny";

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();
            var gameService = this.CreateGameService(gameRepository, userRepository, userId);

            var request = new HttpRequestMessage();
            request.Content = new StringContent("gameType=Skirmish");

            var result = gameService.Queue(request);
            Assert.IsTrue(result.IsSuccessStatusCode);

            var message = this.skirmishGameMessageQueue.GetMessage(new TimeSpan(0, 0, 30));

            Assert.IsNotNull(message);
            Assert.AreEqual(userId, message.UserId);
            this.skirmishGameMessageQueue.DeleteMessage(message);
        }

        [TestMethod]
        public void BadRequestIfQueueWithEmptyUser()
        {
            string username = string.Empty;

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            GameService gameService = this.CreateGameService(gameRepository, userRepository, username);

            var request = new HttpRequestMessage();
            request.Content = new StringContent("gameType=Skirmish");

            var response = gameService.Queue(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("User Id cannot be empty", response.Content.ReadAsString());
        }

        [TestMethod]
        public void BadRequestIfQueueWithNullUser()
        {
            string username = null;

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            GameService gameService = this.CreateGameService(gameRepository, userRepository, username);

            var request = new HttpRequestMessage();
            request.Content = new StringContent("gameType=Skirmish");

            var response = gameService.Queue(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("User Id cannot be empty", response.Content.ReadAsString());
        }

        #endregion

        #region /game/leave/
        [TestMethod]
        public void Leave()
        {
            string userId = "jhonny";
            Guid gameId = Guid.NewGuid();

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userId);            

            var result = gameService.Leave(gameId, null);
            Assert.IsTrue(result.IsSuccessStatusCode);

            var message = this.leaveGameMessageQueue.GetMessage(new TimeSpan(0, 0, 30));

            Assert.IsNotNull(message);
            Assert.AreEqual(message.GameId, gameId);
            Assert.AreEqual(message.UserId, userId);
            this.leaveGameMessageQueue.DeleteMessage(message);
        }

        [TestMethod]
        public void BadRequestIfLeaveWithEmptyUser()
        {
            string userId = string.Empty;
            Guid gameId = Guid.NewGuid();

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userId);

            var response = gameService.Leave(gameId, null);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("User Id cannot be null or empty", response.Content.ReadAsString());
        }

        [TestMethod]
        public void BadRequestIfLeaveWithNullUser()
        {
            string userId = string.Empty;
            Guid gameId = Guid.NewGuid();

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userId);

            var response = gameService.Leave(gameId, null);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("User Id cannot be null or empty", response.Content.ReadAsString());
        }
        #endregion

        [TestMethod]
        public void NewGameService()
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
            var gameService = new GameService();
            Assert.IsTrue(wasSetterCalled);

            FieldInfo fieldInfo = gameService.GetType().GetField("gameRepository", BindingFlags.Instance | BindingFlags.NonPublic);
            object gameRepository = fieldInfo.GetValue(gameService);
            Assert.IsInstanceOfType(gameRepository, typeof(GameRepository));

            fieldInfo = typeof(ServiceBase).GetField("userProvider", BindingFlags.Instance | BindingFlags.NonPublic);
            object userProvider = fieldInfo.GetValue(gameService);
            Assert.IsInstanceOfType(userProvider, typeof(HttpContextUserProvider));
        }

        #region /game/create/
        [TestMethod]
        public void CreateANewGame()
        {
            string userID = "jhonny";

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            this.CreateUser(userRepository, userID);

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userID);

            var response = gameService.Create();

            var id = response.Content.ReadAsString();
            var gameId = Guid.Parse(id);
            Assert.AreNotEqual(Guid.Empty, gameId);

            GameQueue gameQueue = this.gameQueueContainer.Get(gameId.ToString());

            Assert.IsNotNull(gameQueue);
            Assert.AreEqual(1, gameQueue.Users.Count);
            Assert.AreEqual(userID, gameQueue.Users[0].UserId);
            Assert.AreEqual(QueueStatus.Waiting, gameQueue.Status);
        }

        [TestMethod]
        public void BadRequestWhenCreateANewGameWithUnknowUser()
        {
            string userID = "jhonny";

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userID);

            var response = gameService.Create();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(response.Content.ReadAsString().StartsWith("User does not exist"));
        }

        #endregion

        #region /game/join

        [TestMethod]
        public void JoinGame()
        {
            string userID = "jhonny";
            string newUserID = "joe";

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            this.CreateUser(userRepository, userID);
            this.CreateUser(userRepository, newUserID);

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userID);

            var response = gameService.Create();
            var id = response.Content.ReadAsString();
            var gameId = Guid.Parse(id);

            GameService newGameService = this.CreateGameService(gameRepository, userRepository, newUserID);

            var response2 = newGameService.Join(gameId);
            Assert.IsTrue(response2.IsSuccessStatusCode);

            GameQueue gameQueue = this.gameQueueContainer.Get(gameId.ToString());

            Assert.IsNotNull(gameQueue);
            Assert.AreEqual(2, gameQueue.Users.Count);
            Assert.AreEqual(userID, gameQueue.Users[0].UserId);
            Assert.AreEqual(newUserID, gameQueue.Users[1].UserId);

            var friends1 = userRepository.GetFriends(userID);
            var friends2 = userRepository.GetFriends(newUserID);

            Assert.IsTrue(friends1.Any(f => f == newUserID));
            Assert.IsTrue(friends2.Any(f => f == userID));
        }

        [TestMethod]
        public void JoinGameTwice()
        {
            string userID = "jhonny";

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            this.CreateUser(userRepository, userID);

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userID);

            var response = gameService.Create();
            var id = response.Content.ReadAsString();
            var gameId = Guid.Parse(id);

            var response2 = gameService.Join(gameId);
            Assert.IsTrue(response2.IsSuccessStatusCode);

            GameQueue gameQueue = this.gameQueueContainer.Get(gameId.ToString());

            Assert.IsNotNull(gameQueue);
            Assert.AreEqual(1, gameQueue.Users.Count);
            Assert.AreEqual(userID, gameQueue.Users[0].UserId);
        }

        [TestMethod]
        public void BadRequestWhenJoinUnknownGame()
        {
            string userID = "jhonny";

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            this.CreateUser(userRepository, userID);

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userID);

            var gameId = Guid.NewGuid();

            var response = gameService.Join(gameId);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(response.Content.ReadAsString().StartsWith("Game Queue does not exist"));
        }

        #endregion  

        #region /game/start

        [TestMethod]
        public void StartGame()
        {
            string userID = "jhonny";

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            this.CreateUser(userRepository, userID);

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userID);

            var response = gameService.Create();

            var id = response.Content.ReadAsString();
            var gameId = Guid.Parse(id);

            gameService.Start(gameId);

            GameQueue gameQueue = this.gameQueueContainer.Get(gameId.ToString());

            Assert.IsNotNull(gameQueue);
            Assert.AreEqual(QueueStatus.Ready, gameQueue.Status);
            Assert.AreNotEqual(Guid.Empty, gameQueue.GameId);

            Game game = this.gameContainer.Get(gameQueue.GameId.ToString());

            Assert.IsNotNull(game);
            Assert.IsNotNull(game.Users);
            Assert.AreEqual(1, game.Users.Count);
            Assert.AreEqual(userID, game.Users[0].UserId);
        }

        [TestMethod]
        public void BadRequestIfGameQueueDoesNotExistWhenStartGame()
        {
            string userID = "jhonny";

            var userRepository = this.CreateUserRepository();
            var gameRepository = this.CreateGameRepository();

            GameService gameService = this.CreateGameService(gameRepository, userRepository, userID);
            var gameId = Guid.NewGuid();

            var response = gameService.Start(gameId);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.IsTrue(response.Content.ReadAsString().StartsWith("Game Queue does not exist"));
        }

        #endregion

        private GameService CreateGameService(IGameRepository gameRepository, IUserRepository userRepository, string userId)
        {
            return new GameService(gameRepository, userRepository, new StringUserProvider(userId));
        }

        private GameRepository CreateGameRepository()
        {
            CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            this.gameContainer = new AzureBlobContainer<Game>(account, ConfigurationConstants.GamesContainerName + "test" + this.suffix, true);
            this.gameQueueContainer = new AzureBlobContainer<GameQueue>(account, ConfigurationConstants.GamesQueuesContainerName + "test" + this.suffix, true);
            this.skirmishGameMessageQueue = new AzureQueue<SkirmishGameQueueMessage>(account, ConfigurationConstants.SkirmishGameQueue + this.suffix);
            this.leaveGameMessageQueue = new AzureQueue<LeaveGameMessage>(account, ConfigurationConstants.LeaveGameQueue + "test" + this.suffix);
            this.inviteQueue = new AzureQueue<InviteMessage>(account, ConfigurationConstants.InvitesQueue + "test" + this.suffix);

            this.gameContainer.EnsureExist(true);
            this.gameQueueContainer.EnsureExist(true);

            return new GameRepository(this.gameContainer, this.gameQueueContainer, this.skirmishGameMessageQueue, this.leaveGameMessageQueue, this.userContainer, this.inviteQueue);
        }

        private UserRepository CreateUserRepository()
        {
            var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            this.userContainer = new AzureBlobContainer<UserProfile>(account, ConfigurationConstants.UsersContainerName + "test" + this.suffix, true);
            this.userSessionContainer = new AzureBlobContainer<UserSession>(account, ConfigurationConstants.UserSessionsContainerName + "test" + this.suffix, true);
            this.friendContainer = new AzureBlobContainer<Friends>(account, ConfigurationConstants.FriendsContainerName + "test" + this.suffix, true);

            this.userContainer.EnsureExist(true);
            this.userSessionContainer.EnsureExist(true);
            this.friendContainer.EnsureExist(true);

            return new UserRepository(this.userContainer, this.userSessionContainer, this.friendContainer);
        }

        private Game CreateNewGame(IGameRepository gameRepository, params string[] userIds)
        {
            Guid gameID = Guid.NewGuid();

            Game game = new Game() { Id = gameID, ActiveUser = "jhonny", GameActions = new List<GameAction>(), Users = new List<GameUser>() };

            foreach (string userId in userIds)
            {
                GameUser user = new GameUser() { UserId = userId };
                game.Users.Add(user);
            }

            gameRepository.AddOrUpdateGame(game);

            return game;
        }

        private void CreateUser(IUserRepository userRepository, string userId)
        {
            var userProfile = new UserProfile()
            {
                Id = userId,
                DisplayName = userId
            };

            userRepository.AddOrUpdateUser(userProfile);
        }
    }
}