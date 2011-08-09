namespace Microsoft.Samples.SocialGames.Tests
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Transactions;
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
    public class UserServiceTest : ServiceTest
    {
        private static InventoryItem[] initialInventoryItems = new[]
        {
            new InventoryItem { Id = new Guid("1b6e4f64-0eee-4a86-9d7b-9cd667b1ec46"), Name = "Better Weapon 1", SmlIcon = "someimage.jpg", Price = 12, Type = "Tank" },
            new InventoryItem { Id = new Guid("dd9752fd-1117-44df-8bae-3f0e62393600"), Name = "Better Weapon 2", SmlIcon = "someimage.jpg", Price = 4, Type = "Other" },
            new InventoryItem { Id = new Guid("9a4a45cd-da7f-4597-9892-4ec70fa4be97"), Name = "Better Weapon 3", SmlIcon = "someimage.jpg", Price = 7, Type = "Tank" }
        };

        private int suffix;
        private IAzureBlobContainer<UserProfile> userContainer;
        private IAzureBlobContainer<UserSession> userSessionContainer;
        private IAzureBlobContainer<InventoryItem[]> inventoryContainer;

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
            if (this.userContainer != null)
            {
                this.userContainer.DeleteContainer();
            }

            if (this.userSessionContainer != null)
            {
                this.userSessionContainer.DeleteContainer();
            }

            if (this.inventoryContainer != null)
            {
                this.inventoryContainer.DeleteContainer();
            }
        }

        [TestMethod]
        public void Verify()
        {
            var userId = "UoJw5TuD3UGu9Jd8ct2Fm+tVuo4Xl4fYKvGmT7sldz4=";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);
            var request = new HttpRequestMessage();

            var response = userService.Verify(request);

            Assert.AreEqual(userId, response.Content.ReadAsString());
        }

        [TestMethod]
        public void VerifyReturnsErrorIfUserIsNotAuthenticated()
        {
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, string.Empty);
            var request = new HttpRequestMessage();

            var response = userService.Verify(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("The user is not authenticated", response.Content.ReadAsString());
        }

        [TestMethod]
        public void UpdateUserProfileChangeDisplayName()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "Johnny Anderson";
            var newName = "Johnny New Name";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);

            var user = new UserProfile { Id = userId, DisplayName = userName };
            userRepository.AddOrUpdateUser(user);

            var parametersTemplate = "displayName={0}";
            var parameters = string.Format(CultureInfo.InvariantCulture, parametersTemplate, newName);
            
            var request = new HttpRequestMessage
            {
                Content = new StringContent(parameters)
            };
            
            userService.UpdateProfile(request);
            user = userRepository.GetUser(userId);

            Assert.AreEqual(userId, user.Id);
            Assert.AreEqual(newName, user.DisplayName);
        }

        [TestMethod]
        public void UpdateUserProfileChangeCustomizationWhenCustomizationDictionaryIsEmpty()
        {
            var userId = Guid.NewGuid().ToString();
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);
            var inventoryItem1 = initialInventoryItems[0];
            var inventoryItem2 = initialInventoryItems[1];

            var user = new UserProfile { Id = userId, Credits = 100 };
            userRepository.AddOrUpdateUser(user);

            userService.BuyInventory(inventoryItem1.Id, new HttpRequestMessage());
            userService.BuyInventory(inventoryItem2.Id, new HttpRequestMessage());

            var parametersTemplate = "customizationsIds[]={0}&customizationsIds[]={1}";
            var parameters = string.Format(CultureInfo.InvariantCulture, parametersTemplate, inventoryItem1.Id, inventoryItem2.Id);

            var request = new HttpRequestMessage { Content = new StringContent(parameters) };
            userService.UpdateProfile(request);
            user = userRepository.GetUser(userId);

            Assert.AreEqual(userId, user.Id);
            Assert.AreEqual(2, user.Customizations.Count);

            Assert.AreEqual(inventoryItem1.Type, user.Customizations.First().Key);
            Assert.AreEqual(inventoryItem1.Id, user.Customizations.First().Value.Id);
            Assert.AreEqual(inventoryItem1.Type, user.Customizations.First().Value.Type);
            Assert.AreEqual(inventoryItem1.Name, user.Customizations.First().Value.Name);
            Assert.AreEqual(inventoryItem1.Price, user.Customizations.First().Value.Price);
            Assert.AreEqual(inventoryItem1.Type, user.Customizations.First().Value.Type);

            Assert.AreEqual(inventoryItem2.Type, user.Customizations.Last().Key);
            Assert.AreEqual(inventoryItem2.Id, user.Customizations.Last().Value.Id);
            Assert.AreEqual(inventoryItem2.SmlIcon, user.Customizations.Last().Value.SmlIcon);
            Assert.AreEqual(inventoryItem2.Name, user.Customizations.Last().Value.Name);
            Assert.AreEqual(inventoryItem2.Price, user.Customizations.Last().Value.Price);
            Assert.AreEqual(inventoryItem2.Type, user.Customizations.Last().Value.Type);
        }

        [TestMethod]
        public void UpdateUserProfileChangeCustomization()
        {
            var userId = Guid.NewGuid().ToString();
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);
            var inventoryItem1 = initialInventoryItems[0];
            var inventoryItem2 = initialInventoryItems[1];
            var inventoryItem3 = initialInventoryItems[2];

            var user = new UserProfile { Id = userId, Credits = 100 };
            userRepository.AddOrUpdateUser(user);

            userService.BuyInventory(inventoryItem1.Id, new HttpRequestMessage());
            userService.BuyInventory(inventoryItem2.Id, new HttpRequestMessage());
            userService.BuyInventory(inventoryItem3.Id, new HttpRequestMessage());

            var parametersTemplate = "customizationsIds[]={0}&customizationsIds[]={1}";
            var parameters = string.Format(CultureInfo.InvariantCulture, parametersTemplate, inventoryItem1.Id, inventoryItem2.Id);
            var request = new HttpRequestMessage { Content = new StringContent(parameters) };
            userService.UpdateProfile(request);

            parameters = string.Format(CultureInfo.InvariantCulture, parametersTemplate, inventoryItem3.Id, inventoryItem2.Id);
            request = new HttpRequestMessage { Content = new StringContent(parameters) };
            userService.UpdateProfile(request);

            user = userRepository.GetUser(userId);

            Assert.AreEqual(userId, user.Id);
            Assert.AreEqual(2, user.Customizations.Count);

            Assert.AreEqual(inventoryItem3.Type, user.Customizations.First().Key);
            Assert.AreEqual(inventoryItem3.Id, user.Customizations.First().Value.Id);
            Assert.AreEqual(inventoryItem3.SmlIcon, user.Customizations.First().Value.SmlIcon);
            Assert.AreEqual(inventoryItem3.Name, user.Customizations.First().Value.Name);
            Assert.AreEqual(inventoryItem3.Price, user.Customizations.First().Value.Price);
            Assert.AreEqual(inventoryItem3.Type, user.Customizations.First().Value.Type);

            Assert.AreEqual(inventoryItem2.Type, user.Customizations.Last().Key);
            Assert.AreEqual(inventoryItem2.Id, user.Customizations.Last().Value.Id);
            Assert.AreEqual(inventoryItem2.SmlIcon, user.Customizations.Last().Value.SmlIcon);
            Assert.AreEqual(inventoryItem2.Name, user.Customizations.Last().Value.Name);
            Assert.AreEqual(inventoryItem2.Price, user.Customizations.Last().Value.Price);
            Assert.AreEqual(inventoryItem2.Type, user.Customizations.Last().Value.Type);
        }

        [TestMethod]
        public void UpdateUserProfileReturnsErrorIfUserDoesNotExist()
        {
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, "invalid-user");
            var request = new HttpRequestMessage { Content = new StringContent("displayName=john") };

            var response = userService.UpdateProfile(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("User does not exist", response.Content.ReadAsString());
        }

        [TestMethod]
        public void UpdateUserProfileDoesNotChangeIfDisplayNameIsEmpty()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "Johnny Anderson";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);

            var user = new UserProfile { Id = userId, DisplayName = userName };
            userRepository.AddOrUpdateUser(user);

            var request = new HttpRequestMessage { Content = new StringContent("displayName=") };

            userService.UpdateProfile(request);
            user = userRepository.GetUser(userId);

            Assert.AreEqual(userId, user.Id);
            Assert.AreEqual(userName, user.DisplayName);
        }

        [TestMethod]
        public void UpdateUserProfileDoesNotChangeIfDisplayNameParameterIsMissing()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "Johnny Anderson";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);

            var user = new UserProfile { Id = userId, DisplayName = userName };
            userRepository.AddOrUpdateUser(user);

            var request = new HttpRequestMessage { Content = new StringContent(string.Empty) };

            userService.UpdateProfile(request);
            user = userRepository.GetUser(userId);

            Assert.AreEqual(userId, user.Id);
            Assert.AreEqual(userName, user.DisplayName);
        }

        [TestMethod]
        public void BuyItem()
        {
            var userId = "jhonny";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);
            var itemId = initialInventoryItems[0].Id;
            var initialCredit = 20;

            var user = new UserProfile { Id = userId, Credits = initialCredit };
            userRepository.AddOrUpdateUser(user);

            var request = new HttpRequestMessage();
            userService.BuyInventory(itemId, request);

            var updatedUser = userRepository.GetUser(userId);

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(1, updatedUser.Inventory.Count);

            var inventory = updatedUser.Inventory.First();
            
            Assert.AreEqual(itemId, inventory.Id);
            Assert.AreEqual(initialCredit - initialInventoryItems[0].Price, updatedUser.Credits);
        }

        [TestMethod]
        public void BuyItemErrorIfNoEnoughCredits()
        {
            var userId = "jhonny";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);
            var itemId = initialInventoryItems[1].Id;

            var user = new UserProfile { Id = userId, Credits = initialInventoryItems[1].Price - 1 };
            userRepository.AddOrUpdateUser(user);

            var request = new HttpRequestMessage();
            var response = userService.BuyInventory(itemId, request);

            MyAsserts.IsBadRequest(response, "Not enough credits");
        }

        [TestMethod]
        public void BuyItemErrorIfItemDoesNotExist()
        {
            var userId = "jhonny";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);
            var itemId = Guid.NewGuid();

            var user = new UserProfile { Id = userId, Credits = 100 };
            userRepository.AddOrUpdateUser(user);

            var request = new HttpRequestMessage();
            var response = userService.BuyInventory(itemId, request);

            MyAsserts.IsBadRequest(response, string.Format(CultureInfo.InvariantCulture, "Inventory with id '{0}' does not exist", itemId));
        }

        [TestMethod]
        public void BuyItemErrorIfItemIsEmpty()
        {
            var userId = "jhonny";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);
            var itemId = Guid.Empty;

            var user = new UserProfile { Id = userId, Credits = 100 };
            userRepository.AddOrUpdateUser(user);

            var request = new HttpRequestMessage();
            var response = userService.BuyInventory(itemId, request);

            MyAsserts.IsBadRequest(response, "InventoryId cannot be empty");
        }

        [TestMethod]
        public void BuyItemErrorIfDuplicatedItem()
        {
            var userId = "jhonny";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);
            var itemId = initialInventoryItems[0].Id;

            var user = new UserProfile { Id = userId, Credits = 100 };
            userRepository.AddOrUpdateUser(user);

            var request = new HttpRequestMessage();
            userService.BuyInventory(itemId, request);
            
            var response = userService.BuyInventory(itemId, request);

            MyAsserts.IsBadRequest(response, "Item already in inventory");
        }

        [TestMethod]
        public void BuyItemErrorIfUnknownUser()
        {
            var userId = "jhonny";
            var userRepository = this.CreateUserRepository();
            var inventoryRepository = this.CreateInventoryRepository();
            var statisticsProvider = this.CreateStatisticsRepository();
            var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);
            var itemId = Guid.NewGuid();

            HttpRequestMessage request = new HttpRequestMessage();
            request.Content = new StringContent("image=image.jpg&price=30");

            var response = userService.BuyInventory(itemId, request);
            MyAsserts.IsBadRequest(response, "User does not exist");
        }

        [TestMethod]
        public void LeaderboardTop10()
        {
            using (var ts = new TransactionScope())
            {
                var userId = "testuser_10";
                var userRepository = this.CreateUserRepository();
                var inventoryRepository = this.CreateInventoryRepository();
                var statisticsProvider = this.CreateStatisticsRepository();

                this.BulkInsertTestData(statisticsProvider);

                var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);

                var response = userService.Leaderboard(10);
                var serializer = new JavaScriptSerializer();
                var boards = serializer.Deserialize<Board[]>(response.Content.ReadAsString());

                Assert.AreEqual(6, boards.Count());

                foreach (var board in boards)
                {
                    Assert.AreEqual(10, board.Scores.Count());
                }
            }
        }

        [TestMethod]
        public void LeaderboardUserFocused()
        {
            using (var ts = new TransactionScope())
            {
                var userId = "testuser_10";
                var userRepository = this.CreateUserRepository();
                var inventoryRepository = this.CreateInventoryRepository();
                var statisticsProvider = this.CreateStatisticsRepository();

                this.BulkInsertTestData(statisticsProvider);

                var userService = this.CreateUserService(userRepository, inventoryRepository, statisticsProvider, userId);

                var response = userService.LeaderboardWithFocus(userId, 2);
                var serializer = new JavaScriptSerializer();
                var boards = serializer.Deserialize<Board[]>(response.Content.ReadAsString());

                Assert.AreEqual(6, boards.Count());

                foreach (var board in boards)
                {
                    Assert.AreEqual(5, board.Scores.Count());
                    Assert.IsNotNull(board.Scores.FirstOrDefault(s => s.UserId == userId));
                }
            }
        }

        private void BulkInsertTestData(IStatisticsRepository repository)
        {
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

        private UserService CreateUserService(IUserRepository userRepository, IInventoryRepository inventoryRepository, IStatisticsRepository statisticsProvider, string userId)
        {
            return new UserService(
                userRepository,
                inventoryRepository, 
                statisticsProvider,
                new StringUserProvider(userId));
        }

        private UserRepository CreateUserRepository()
        {
            var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            this.userContainer = new AzureBlobContainer<UserProfile>(account, ConfigurationConstants.UsersContainerName + "test" + this.suffix, true);
            this.userSessionContainer = new AzureBlobContainer<UserSession>(account, ConfigurationConstants.UserSessionsContainerName + "test" + this.suffix, true);

            this.userContainer.EnsureExist();
            this.userSessionContainer.EnsureExist(true);

            return new UserRepository(this.userContainer, this.userSessionContainer);
        }

        private InventoryRepository CreateInventoryRepository()
        {
            var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            this.inventoryContainer = new AzureBlobContainer<InventoryItem[]>(account, ConfigurationConstants.InventoryContainerName + "test" + this.suffix, true);
            this.inventoryContainer.EnsureExist(true);
            this.inventoryContainer.Save(ConfigurationConstants.InventoryItemsBlobName, initialInventoryItems);

            return new InventoryRepository(this.inventoryContainer);
        }

        private StatisticsRepository CreateStatisticsRepository()
        {
            return new StatisticsRepository("Data Source=.\\SQLEXPRESS;Initial Catalog=SocialGames;Integrated Security=True");
        }
    }
}