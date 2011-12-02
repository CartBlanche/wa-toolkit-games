namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames.Entities;

    public interface IGameActionStatisticsQueue
    {
        void Add(GameAction gameAction);
    }
}
