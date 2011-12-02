namespace Microsoft.Samples.SocialGames.Tests.Mocks
{
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class MockStatisticsRepository : IStatisticsRepository
    {
        private UserStats stats;

        public UserStats Retrieve(string userId)
        {
            return this.stats;
        }

        public void Save(UserStats stats)
        {
            this.stats = stats;
        }

        public IEnumerable<Board> GenerateLeaderboard(int count)
        {
            return null;
        }

        public IEnumerable<Board> GenerateLeaderboard(string focusUserId, int focusCount)
        {
            return null;
        }
    }
}
