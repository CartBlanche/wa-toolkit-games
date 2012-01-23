namespace Microsoft.Samples.SocialGames.Repositories
{
    using Microsoft.Samples.SocialGames.Entities;

    public interface IGameActionStatisticsQueue
    {
        void Add(GameAction gameAction);
    }
}
