namespace Microsoft.Samples.SocialGames.Worker.Tests.Commands
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Worker.Commands;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class InviteCommandTests
    {
        private InviteCommand command;
        private INotificationRepository notificationRepository;
        private UserRepository userRepository;

        private AzureBlobContainer<NotificationStatus> notificationContainer;
        private AzureBlobContainer<UserProfile> userContainer;
        private AzureBlobContainer<UserSession> userSessionContainer;
        private AzureBlobContainer<Friends> friendsContainer;

        [TestInitialize]
        public void Setup()
        {
            int suffix = (new Random()).Next(10000);
            var account = CloudStorageAccount.DevelopmentStorageAccount;

            this.notificationContainer = new AzureBlobContainer<NotificationStatus>(account, ConfigurationConstants.NotificationsContainerName + "test" + suffix, true);
            this.userContainer = new AzureBlobContainer<UserProfile>(account, ConfigurationConstants.UsersContainerName + "test" + suffix, true);
            this.userSessionContainer = new AzureBlobContainer<UserSession>(account, ConfigurationConstants.UserSessionsContainerName + "test" + suffix, true);
            this.friendsContainer = new AzureBlobContainer<Friends>(account, ConfigurationConstants.FriendsContainerName + "test" + suffix, true);

            this.notificationRepository = new NotificationRepository(this.notificationContainer);
            this.userRepository = new UserRepository(this.userContainer, this.userSessionContainer, this.friendsContainer);
            this.userRepository.Initialize();
            this.command = new InviteCommand(this.notificationRepository, this.userRepository);
        }

        [TestCleanup]
        public void Teardown()
        {
            if (this.notificationContainer != null)
            {
                this.notificationContainer.DeleteContainer();
            }

            if (this.userContainer != null)
            {
                this.userContainer.DeleteContainer();
            }

            if (this.userSessionContainer != null)
            {
                this.userSessionContainer.DeleteContainer();
            }

            if (this.friendsContainer != null)
            {
                this.friendsContainer.DeleteContainer();
            }
        }

        [TestMethod]
        public void SendInvite()
        {
            var userId = "johnny";
            var invitedUserId = "mary";
            var timestamp = DateTime.Now;
            var gameQueueId = Guid.NewGuid();
            var message = "New Invite to New Game";
            var url = "http://127.0.0.1:81/TicTacToe";

            var data = new Dictionary<string, object>()
            {
                    { "userId", userId },
                    { "invitedUserId", invitedUserId },
                    { "gameQueueId", gameQueueId },
                    { "timestamp", timestamp },
                    { "message", message },
                    { "url", url }
            };

            this.command.Do(data);

            var result = this.notificationRepository.GetNotificationStatus(invitedUserId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Notifications.Count);

            var notification = result.Notifications[0];

            Assert.AreEqual(userId, notification.SenderId);
            Assert.AreEqual(userId, notification.SenderName);
            Assert.AreEqual("Invite", notification.Type);
            Assert.AreEqual(gameQueueId.ToString(), notification.Data);
            Assert.AreEqual(message, notification.Message);
            Assert.AreEqual(url, notification.Url);
        }
    }
}
