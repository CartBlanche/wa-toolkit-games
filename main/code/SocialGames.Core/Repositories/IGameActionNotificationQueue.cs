namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames.Entities;

    public interface IGameActionNotificationQueue
    {
        void Add(GameAction gameAction);
    }
}
