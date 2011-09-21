namespace Microsoft.Samples.SocialGames.GamePlay.Services
{
    using System;
    using System.Collections.Generic;
    using System.Json;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using System.Web.Mvc;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.GamePlay.Extensions;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.WindowsAzure;

    public class EventService : ServiceBase, IEventService
    {
        private IGameActionNotificationQueue notificationQueue;
        private IGameActionStatisticsQueue statisticsQueue;

        public EventService()
            : this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"))
        { 
        }

        public EventService(CloudStorageAccount account)
            : this(new GameActionNotificationQueue(account), new GameActionStatisticsQueue(account), new HttpContextUserProvider())
        {
        }

        public EventService(IGameActionNotificationQueue notificationQueue, IGameActionStatisticsQueue statisticsQueue, IUserProvider userProvider)
            : base(userProvider)
        {
            this.notificationQueue = notificationQueue;
            this.statisticsQueue = statisticsQueue;
        }

        [Authorize]
        public HttpResponseMessage PostEvent(string topic, HttpRequestMessage request)
        {
            var formContent = GetFormContent(request);

            // Command Type
            int commandType;

            try
            {
                commandType = int.Parse(formContent.type.Value);
            }
            catch
            {
                return BadRequest("Invalid type parameter");
            }

            if (topic != "notifications" && topic != "statistics")
            {
                return BadRequest("Invalid topic parameter");
            }

            // Command Data
            var jsonCommandData = (JsonObject)(formContent.commandData ?? null);
            IDictionary<string, object> commandData = null;

            if (jsonCommandData != null)
            {
                commandData = jsonCommandData.ToDictionary();
            }

            try
            {
                // Add gameAction
                var gameAction = new GameAction
                {
                    Id = Guid.NewGuid(),
                    Type = commandType,
                    CommandData = commandData,
                    UserId = this.CurrentUserId,
                    Timestamp = DateTime.UtcNow
                };

                if (topic == "notifications")
                {
                    this.notificationQueue.Add(gameAction);
                }
                else if (topic == "statistics")
                {
                    this.statisticsQueue.Add(gameAction);
                }

                return SuccessResponse;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
