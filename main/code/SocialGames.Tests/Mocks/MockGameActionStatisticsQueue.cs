namespace Microsoft.Samples.SocialGames.Tests.Mocks
{
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class MockGameActionStatisticsQueue : IGameActionStatisticsQueue
    {
        private GameAction gameAction;

        public void Add(GameAction gameAction)
        {
            this.gameAction = gameAction;
        }

        public GameAction GetQueuedGameAction()
        {
            return this.gameAction;
        }
    }
}
