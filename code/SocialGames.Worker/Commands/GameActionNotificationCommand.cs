namespace Microsoft.Samples.SocialGames.Worker.Commands
{
    using System;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class GameActionNotificationCommand : GameActionCommand
    {
        private INotificationRepository notificationRepository;

        public GameActionNotificationCommand(INotificationRepository notificationRepository)
        {
            this.notificationRepository = notificationRepository;
        }

        public override void Do(GameAction gameAction)
        {
            // TODO Any code?
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
