namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames.Entities;

    public interface INotificationRepository
    {
        void AddGlobalNotification(Notification notification);

        void AddNotification(string userId, Notification notification);

        NotificationStatus GetNotificationStatus(string userId);

        NotificationStatus GetGlobalNotificationStatus();
    }
}
