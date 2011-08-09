namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure;

    public class NotificationRepository : INotificationRepository
    {
        private readonly IAzureBlobContainer<NotificationStatus> notificationsContainer;

        public NotificationRepository()
            : this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"))
        { 
        }

        public NotificationRepository(CloudStorageAccount account)
            : this(account, ConfigurationConstants.NotificationsContainerName)
        { 
        }

        public NotificationRepository(CloudStorageAccount account, string notificationsContainerName)
            : this(new AzureBlobContainer<NotificationStatus>(account, notificationsContainerName, true))
        {
        }

        public NotificationRepository(IAzureBlobContainer<NotificationStatus> notificationsContainer)
        {
            if (notificationsContainer == null)
            {
                throw new ArgumentNullException("notificationsContainer");
            }

            this.notificationsContainer = notificationsContainer;
            this.notificationsContainer.EnsureExist(true);
        }

        public void AddNotification(string userId, Notification notification)
        {
            var status = this.notificationsContainer.Get(userId);

            if (status == null)
            {
                status = new NotificationStatus();
                status.Notifications = new List<Notification>();
            }

            status.Notifications.Add(notification);

            var toremove = status.Notifications.Where(n => notification.DateTime.Subtract(n.DateTime).TotalSeconds >= ConfigurationConstants.NotificationTimeInterval).ToList();

            foreach (var item in toremove)
            {
                status.Notifications.Remove(item);
            }

            this.notificationsContainer.Save(userId, status);
        }

        public void AddGlobalNotification(Notification notification)
        {
            throw new NotImplementedException();
        }

        public NotificationStatus GetNotificationStatus(string userId)
        {
            return this.notificationsContainer.Get(userId);
        }

        public NotificationStatus GetGlobalNotificationStatus()
        {
            throw new NotImplementedException();
        }
    }
}