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
    public class GameActionNotificationCommandTests
    {
        private GameActionNotificationCommand command;
        private INotificationRepository repository;

        [TestInitialize]
        public void Setup()
        {
            var account = CloudStorageAccount.DevelopmentStorageAccount;
            var container = new AzureBlobContainer<NotificationStatus>(account, ConfigurationConstants.NotificationsContainerName + "test", true);
            this.repository = new NotificationRepository(container);
            this.command = new GameActionNotificationCommand(this.repository);
        }

        [TestMethod]
        public void SendShotCommand()
        {
            string userId = Guid.NewGuid().ToString();

            GameAction gameAction = new GameAction()
            {
                Type = GameActionType.Shot,
                Id = Guid.NewGuid(),
                CommandData = new Dictionary<string, object>() { { "shootedId", userId } }
            };

            this.command.Do(gameAction);

            var result = this.repository.GetNotificationStatus(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Notifications.Count);
        }
    }
}
