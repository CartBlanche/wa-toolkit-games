namespace Microsoft.Samples.SocialGames.Repositories
{
    using Microsoft.Samples.SocialGames.Entities;

    public interface IGameActionNotificationQueue
    {
        void Add(GameAction gameAction);
    }
}
