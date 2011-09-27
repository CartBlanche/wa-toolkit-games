namespace Microsoft.Samples.SocialGames.Tests.Repositories
{
    using System;
    using System.Net;
    using System.Reflection;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class UserRepositoryTest : RepositoryTest
    {
        [ClassInitialize]
        public static void InitializeWindowsAzureStorageEmulator(TestContext context)
        {
            new EnsureWindowsAzureStorageEmulatorIsRunning().DoIt();
        }

        [TestMethod]
        public void GetUserReferenceTest()
        {
            var target = new UserRepository(CloudStorageAccount.DevelopmentStorageAccount);
            var userID = Guid.NewGuid().ToString();
            var userFirstVersion = new UserProfile() { Id = userID, DisplayName = "John" };
            target.AddOrUpdateUser(userFirstVersion);
            string address = target.GetUserReference(userID, TimeSpan.FromSeconds(10));
            var webClient = new WebClient();
            string data = webClient.DownloadString(address);

            var serialized = this.Serialized(userFirstVersion, true);

            Assert.AreEqual("sgusers" + serialized, data);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Net.WebException))]
        public void GetUserReferenceTimeOutTest()
        {
            UserRepository target = new UserRepository(CloudStorageAccount.DevelopmentStorageAccount);
            string userID = Guid.NewGuid().ToString();
            UserProfile userFirstVersion = new UserProfile() { Id = userID, DisplayName = "John" };
            target.AddOrUpdateUser(userFirstVersion);
            TimeSpan timeSpan = TimeSpan.FromSeconds(1);
            string address = target.GetUserReference(userID, timeSpan);
            System.Threading.Thread.Sleep(timeSpan.Add(TimeSpan.FromSeconds(1)));
            System.Net.WebClient webClient = new System.Net.WebClient();
            string data = webClient.DownloadString(address);
        }

        [TestMethod]
        public void GetUserTest()
        {
            UserRepository target = new UserRepository(CloudStorageAccount.DevelopmentStorageAccount);
            Assert.IsNull(target.GetUser(Guid.NewGuid().ToString()));
        }

        [TestMethod]
        public void AddOrUpdateUserTest()
        {
            UserRepository target = new UserRepository(CloudStorageAccount.DevelopmentStorageAccount);
            string userID = Guid.NewGuid().ToString();
            UserProfile userFirstVersion = new UserProfile() { Id = userID, DisplayName = "John" };
            target.AddOrUpdateUser(userFirstVersion);
            Assert.AreEqual(userFirstVersion, target.GetUser(userID));

            UserProfile otherUser = new UserProfile() { Id = Guid.NewGuid().ToString(), DisplayName = "Peter" };
            target.AddOrUpdateUser(otherUser);
            Assert.AreEqual(otherUser, target.GetUser(otherUser.Id));

            UserProfile userSecondVersion = new UserProfile() { Id = userID, DisplayName = "Johny" };
            Assert.AreEqual(userSecondVersion, target.GetUser(userID));
        }

        [TestMethod]
        public void AddOrUpdateUserSessionTest()
        {
            UserRepository target = new UserRepository(CloudStorageAccount.DevelopmentStorageAccount);
            IAzureBlobContainer<UserSession> userSessionContainer = (IAzureBlobContainer<UserSession>)
                target.GetType().GetField("userSessionContainer", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);

            UserSession userSessionFirstVersion = new UserSession() { UserId = "johnny", ActiveGameQueueId = Guid.NewGuid() };
            target.AddOrUpdateUserSession(userSessionFirstVersion);
            var result = userSessionContainer.Get(userSessionFirstVersion.UserId);
            Assert.AreEqual(userSessionFirstVersion, result);

            UserSession userSessionSecondVersion = new UserSession() { UserId = userSessionFirstVersion.UserId, ActiveGameQueueId = Guid.NewGuid() };
            target.AddOrUpdateUserSession(userSessionSecondVersion);
            result = userSessionContainer.Get(userSessionFirstVersion.UserId);
            Assert.AreEqual(userSessionSecondVersion, result);
            Assert.AreNotEqual(userSessionFirstVersion, result);
        }

        [TestMethod]
        public void FailedAddOrUpdateUserSessionTest()
        {
            UserRepository target = new UserRepository(CloudStorageAccount.DevelopmentStorageAccount);

            UserSession userSessionFirstVersion = new UserSession() { UserId = null, ActiveGameQueueId = Guid.NewGuid() };
            ExceptionAssert.ShouldThrow<ArgumentException>(() => target.AddOrUpdateUserSession(userSessionFirstVersion));

            UserSession userSessionSecondVersion = new UserSession() { UserId = string.Empty, ActiveGameQueueId = Guid.NewGuid() };
            ExceptionAssert.ShouldThrow<ArgumentException>(() => target.AddOrUpdateUserSession(userSessionSecondVersion));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddOrUpdateUserWithEmptyIDTest()
        {
            UserRepository target = new UserRepository(CloudStorageAccount.DevelopmentStorageAccount);
            target.AddOrUpdateUser(new UserProfile() { Id = string.Empty });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetUserWithEmptyIdTest()
        {
            UserRepository target = new UserRepository(CloudStorageAccount.DevelopmentStorageAccount);
            target.GetUser(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOrUpdateUserWithNullUserTest()
        {
            UserRepository target = new UserRepository(CloudStorageAccount.DevelopmentStorageAccount);
            target.AddOrUpdateUser(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserRepositoryConstructorWithNullAccount()
        {
            new UserRepository(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserRepositoryConstructorWithNullAccountAndContainer()
        {
            new UserRepository(null, "userContainer", "userSessionContainer", "friendContainer");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserRepositoryConstructorWithNullUserContainerName()
        {
            new UserRepository(CloudStorageAccount.DevelopmentStorageAccount, null, "userSessionContainer", "friendContainer");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserRepositoryConstructorWithNullUserSessionContainerName()
        {
            new UserRepository(CloudStorageAccount.DevelopmentStorageAccount, "userContainer", null, "friendContainer");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserRepositoryConstructorWithEmptyUserContainerName()
        {
            new UserRepository(CloudStorageAccount.DevelopmentStorageAccount, string.Empty, "userSessionContainer", "friendContainer");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserRepositoryConstructorWithEmptyUserSessionContainerName()
        {
            new UserRepository(CloudStorageAccount.DevelopmentStorageAccount, "userContainer", string.Empty, "friendContainer");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserRepositoryConstructorWithNullUserContainer()
        {
            new UserRepository(null, new AzureBlobContainer<UserSession>(CloudStorageAccount.DevelopmentStorageAccount), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserRepositoryConstructorWithNullUserSessionContainer()
        {
            new UserRepository(new AzureBlobContainer<UserProfile>(CloudStorageAccount.DevelopmentStorageAccount), null, null);
        }

        [TestMethod]
        public void UserRepositoryDefaultConstructor()
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

            new UserRepository();
            Assert.IsTrue(wasSetterCalled);
        }
    }
}