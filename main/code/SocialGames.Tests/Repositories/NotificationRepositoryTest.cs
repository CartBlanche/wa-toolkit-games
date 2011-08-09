namespace Microsoft.Samples.SocialGames.Tests.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    
    [TestClass]
    public class NotificationRepositoryTest : RepositoryTest
    {
        private CloudStorageAccount cloudStorageAccount;
        private AzureBlobContainer<NotificationStatus> notificationsContainer;
        private NotificationRepository notificationRepository;

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
            this.notificationsContainer = new AzureBlobContainer<NotificationStatus>(this.cloudStorageAccount, ConfigurationConstants.NotificationsContainerName + "test", true);
            this.notificationRepository = new NotificationRepository(this.notificationsContainer);
        }

        [TestMethod]
        public void AddNewSimpleNotification()
        {
            DateTime datetime = DateTime.UtcNow;

            Notification notification = new Notification()
            {
                Id = Guid.NewGuid(),
                Message = "New Notification",
                Type = "Type",
                DateTime = datetime
            };

            string userId = Guid.NewGuid().ToString();

            this.notificationRepository.AddNotification(userId, notification);

            var status = this.notificationsContainer.Get(userId);

            Assert.IsNotNull(status);

            Assert.AreEqual(1, status.Notifications.Count);
            Assert.AreEqual(notification.Id, status.Notifications[0].Id);
            Assert.AreEqual(notification.Message, status.Notifications[0].Message);
            Assert.AreEqual(notification.Type, status.Notifications[0].Type);
        }

        [TestMethod]
        public void AddTwoSimpleNotifications()
        {
            DateTime datetime = DateTime.UtcNow;

            Notification notification = new Notification()
            {
                Id = Guid.NewGuid(),
                Message = "New Notification",
                Type = "Type",
                DateTime = datetime
            };

            Notification notification2 = new Notification()
            {
                Id = Guid.NewGuid(),
                Message = "New Notification",
                Type = "Type",
                DateTime = datetime.AddSeconds(1)
            };

            string userId = Guid.NewGuid().ToString();

            this.notificationRepository.AddNotification(userId, notification);
            this.notificationRepository.AddNotification(userId, notification2);

            var status = this.notificationsContainer.Get(userId);

            Assert.IsNotNull(status);

            Assert.AreEqual(2, status.Notifications.Count);

            Assert.AreEqual(notification.Id, status.Notifications[0].Id);
            Assert.AreEqual(notification.Message, status.Notifications[0].Message);
            Assert.AreEqual(notification.Type, status.Notifications[0].Type);

            Assert.AreEqual(notification2.Id, status.Notifications[1].Id);
            Assert.AreEqual(notification2.Message, status.Notifications[1].Message);
            Assert.AreEqual(notification2.Type, status.Notifications[1].Type);
        }

        [TestMethod]
        public void AddTwoSimpleNotificationsInTwoMinutes()
        {
            DateTime datetime = DateTime.UtcNow;

            Notification notification = new Notification()
            {
                Id = Guid.NewGuid(),
                Message = "New Notification",
                Type = "Type",
                DateTime = datetime
            };

            Notification notification2 = new Notification()
            {
                Id = Guid.NewGuid(),
                Message = "New Notification",
                Type = "Type",
                DateTime = datetime.AddMinutes(2)
            };

            string userId = Guid.NewGuid().ToString();

            this.notificationRepository.AddNotification(userId, notification);
            this.notificationRepository.AddNotification(userId, notification);
            this.notificationRepository.AddNotification(userId, notification);
            this.notificationRepository.AddNotification(userId, notification2);

            var status = this.notificationsContainer.Get(userId);

            Assert.IsNotNull(status);

            Assert.AreEqual(1, status.Notifications.Count);

            Assert.AreEqual(notification2.Id, status.Notifications[0].Id);
            Assert.AreEqual(notification2.Message, status.Notifications[0].Message);
            Assert.AreEqual(notification2.Type, status.Notifications[0].Type);
        }
    }
}