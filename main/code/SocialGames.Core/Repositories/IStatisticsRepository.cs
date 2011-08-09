namespace Microsoft.Samples.SocialGames.Repositories
{
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Entities;

    public interface IStatisticsRepository
    {
        UserStats Retrieve(string userId);

        void Save(UserStats stats);

        IEnumerable<Board> GenerateLeaderboard(int count);

        IEnumerable<Board> GenerateLeaderboard(string focusUserId, int focusCount);
    }
}