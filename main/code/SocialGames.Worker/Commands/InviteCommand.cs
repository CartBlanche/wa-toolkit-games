namespace Microsoft.Samples.SocialGames.Worker.Commands
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.JobEngine;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class InviteCommand : ICommand
    {
        private INotificationRepository notificationRepository;
        private IUserRepository userRepository;

        public InviteCommand()
            : this(new NotificationRepository(), new UserRepository())
        {
        }

        public InviteCommand(INotificationRepository notificationRepository, IUserRepository userRepository)
        {
            this.notificationRepository = notificationRepository;
            this.userRepository = userRepository;
        }

        public void Do(IDictionary<string, object> context)
        {
            var userId = context["userId"].ToString();
            var gameQueueid = (Guid)context["gameQueueId"];
            var invitedUserId = context["invitedUserId"].ToString();
            var timestamp = (DateTime)context["timestamp"];
            var message = context["message"].ToString();
            var url = context.ContainsKey("url") ? context["url"].ToString() : null;

            var profile = this.userRepository.GetUser(userId);

            var notification = new Notification()
            {
                Id = Guid.NewGuid(),
                DateTime = timestamp,
                Message = message,
                Data = gameQueueid.ToString(),
                Url = url,
                Type = "Invite",                
                SenderId = userId,
                SenderName = profile != null && string.IsNullOrEmpty(profile.DisplayName) ? profile.DisplayName : userId
            };

            if (profile != null && profile.DisplayName != null)
            {
                notification.SenderName = profile.DisplayName;
            }
            else
            {
                notification.SenderName = userId;
            }

            this.notificationRepository.AddNotification(invitedUserId, notification);
        }
    }
}
