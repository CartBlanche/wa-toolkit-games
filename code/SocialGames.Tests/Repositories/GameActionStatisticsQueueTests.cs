namespace Microsoft.Samples.SocialGames.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;

    [TestClass]
    public class GameActionStatisticsQueueTests
    {
        private int suffix;
        private AzureQueue<GameActionStatisticsMessage> gameActionStatisticsQueue;

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
            if (this.gameActionStatisticsQueue != null)
            {
                this.gameActionStatisticsQueue.DeleteQueue();
            }
        }

        [TestMethod]
        public void SendShot()
        {
            var queue = this.CreateQueue();

            var action = new GameAction()
            {
                Id = Guid.NewGuid(),
                UserId = "johnny",
                CommandData = new Dictionary<string, object>() { { "name", "peter" }, { "point", 1 } }
            };

            queue.Add(action);

            var message = this.gameActionStatisticsQueue.GetMessage(new TimeSpan(10000));

            Assert.IsNotNull(message);
            Assert.IsNotNull(message.GameAction);
            Assert.IsNotNull(message.GameAction.CommandData);

            Assert.AreEqual(action.Id, message.GameAction.Id);
            Assert.AreEqual(action.UserId, message.GameAction.UserId);
            Assert.AreEqual("peter", action.CommandData["name"]);
            Assert.AreEqual(1, action.CommandData["point"]);
        }

        private GameActionStatisticsQueue CreateQueue()
        {
            CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            this.gameActionStatisticsQueue = new AzureQueue<GameActionStatisticsMessage>(account, ConfigurationConstants.GameActionStatisticsQueue + "test" + this.suffix);

            return new GameActionStatisticsQueue(this.gameActionStatisticsQueue);
        }
    }
}
