namespace Microsoft.Samples.SocialGames.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Web.Services;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;

    [TestClass]
    public class EventServiceTest
    {
        private int suffix;
        private AzureQueue<GameActionNotificationMessage> notificationsQueue;
        private AzureQueue<GameActionStatisticsMessage> statisticsQueue;

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
            if (this.notificationsQueue != null)
            {
                this.notificationsQueue.DeleteQueue();
            }

            if (this.statisticsQueue != null)
            {
                this.statisticsQueue.DeleteQueue();
            }
        }

        [TestMethod]
        public void SendSimpleGameActionToNotificationsQueue()
        {
            EventService service = this.CreateEventService();
 
            var request = new HttpRequestMessage();
            request.Content = new StringContent("type=2&commandData[param1]=one&commandData[param2]=two");
            request.Content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";

            var response = service.PostEvent("notifications", request);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var message = this.notificationsQueue.GetMessage();

            Assert.IsNotNull(message);
            Assert.AreEqual(2, message.GameAction.Type);
            Assert.AreEqual("one", message.GameAction.CommandData["param1"]);
            Assert.AreEqual("two", message.GameAction.CommandData["param2"]);
        }

        [TestMethod]
        public void SendSimpleGameActionToStatisticsQueue()
        {
            EventService service = this.CreateEventService();

            var request = new HttpRequestMessage();
            request.Content = new StringContent("type=2&commandData[param1]=one&commandData[param2]=two");
            request.Content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";

            var response = service.PostEvent("statistics", request);

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var message = this.statisticsQueue.GetMessage();

            Assert.IsNotNull(message);
            Assert.AreEqual(2, message.GameAction.Type);
            Assert.AreEqual("one", message.GameAction.CommandData["param1"]);
            Assert.AreEqual("two", message.GameAction.CommandData["param2"]);
        }

        [TestMethod]
        public void BadRequestWhenUnknownTopic()
        {
            EventService service = this.CreateEventService();

            var request = new HttpRequestMessage();
            request.Content = new StringContent("type=2&commandData[param1]=one&commandData[param2]=two");
            request.Content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";

            var response = service.PostEvent("any", request);

            Assert.IsNotNull(response);
            Assert.AreEqual("Invalid topic parameter", response.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void BadRequestWhenInvalidType()
        {
            EventService service = this.CreateEventService();

            var request = new HttpRequestMessage();
            request.Content = new StringContent("type=Shot&commandData[param1]=one&commandData[param2]=two");
            request.Content.Headers.ContentType.MediaType = "application/x-www-form-urlencoded";

            var response = service.PostEvent("any", request);

            Assert.IsNotNull(response);
            Assert.AreEqual("Invalid type parameter", response.Content.ReadAsStringAsync().Result);
        }

        private EventService CreateEventService()
        {
            CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            this.notificationsQueue = new AzureQueue<GameActionNotificationMessage>(account, ConfigurationConstants.GameActionNotificationsQueue + "test" + this.suffix);
            this.statisticsQueue = new AzureQueue<GameActionStatisticsMessage>(account, ConfigurationConstants.GameActionStatisticsQueue + "test" + this.suffix);

            return new EventService(new GameActionNotificationQueue(this.notificationsQueue), new GameActionStatisticsQueue(this.statisticsQueue), new StringUserProvider("test"));
        }
    }
}
