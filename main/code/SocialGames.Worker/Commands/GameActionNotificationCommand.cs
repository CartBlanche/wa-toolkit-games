namespace Microsoft.Samples.SocialGames.Worker.Commands
{
    using System;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class GameActionNotificationCommand : GameActionCommand
    {
        private INotificationRepository notificationRepository;

        public GameActionNotificationCommand()
            : this(new NotificationRepository())
        {
        }

        public GameActionNotificationCommand(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        public override void Do(GameAction gameAction)
        {
            if (gameAction.Type == GameActionType.Shot)
            {
                var shootedId = (string)gameAction.CommandData["shootedId"];

                if (!string.IsNullOrEmpty(shootedId) && shootedId != "null")
                {
                    var notification = this.NewAchievement("You've been shot!!!");
                    this.notificationRepository.AddNotification(shootedId, notification);

                    if (!string.IsNullOrEmpty(gameAction.UserId))
                    {
                        notification = this.NewAchievement("You've shot " + shootedId);
                        this.notificationRepository.AddNotification(gameAction.UserId, notification);
                    }
                }
            }
        }

        private Notification NewAchievement(string message)
        {
            return new Notification()
            {
                Id = Guid.NewGuid(),
                DateTime = DateTime.UtcNow,
                Message = message,
                Type = "Achievement"
            };
        }
    }
}
