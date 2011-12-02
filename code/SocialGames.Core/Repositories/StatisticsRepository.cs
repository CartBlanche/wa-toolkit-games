namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Security.Permissions;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class StatisticsRepository : IStatisticsRepository
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public StatisticsRepository(string nameOrConnectionString)
        {
            var cn = string.Empty;

            try
            {
                cn = RoleEnvironment.GetConfigurationSettingValue(nameOrConnectionString);
            }
            catch
            {
            }

            if (string.IsNullOrEmpty(cn))
            {
                this.NameOrConnectionString = nameOrConnectionString;
            }
            else
            {
                this.NameOrConnectionString = cn;
            }
        }

        public string NameOrConnectionString { get; set; }

        public UserStats Retrieve(string userId)
        {
            UserStats stats = null;

            using (var db = new StatisticsContext(this.NameOrConnectionString))
            {
                stats = db.Statistics.FirstOrDefault(e => e.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
            }
            
            return stats;
        }

        public void Save(UserStats stats)
        {
            using (var db = new StatisticsContext(this.NameOrConnectionString))
            {
                var old = db.Statistics.FirstOrDefault(e => e.UserId.Equals(stats.UserId, StringComparison.OrdinalIgnoreCase));

                if (old != null)
                {
                    old.Victories = stats.Victories;
                    old.Defeats = stats.Defeats;
                    old.GameCount = stats.GameCount;
                }
                else
                {
                    db.Statistics.Add(stats);
                }

                db.SaveChanges();
            }
        }

        public IEnumerable<Board> GenerateLeaderboard(int count)
        {
            return this.GenerateLeaderboard(string.Empty, count);
        }

        public IEnumerable<Board> GenerateLeaderboard(string focusUserId, int focusCount)
        {
            var boards = new List<Board>();
            var boardNames = new string[] { "Victories", "Defeats", "GameCount" };
            int id = 0;

            using (var db = new StatisticsContext(this.NameOrConnectionString))
            {
                foreach (var boardName in boardNames)
                {
                    var board = new Board()
                    {
                        Id = ++id,
                        Name = boardName,
                        Scores = null
                    };

                    board.Scores = db.Database.SqlQuery<Score>(
                            "GenerateBoard @boardName, @count, @focusUserId",
                            new SqlParameter("boardName", boardName),
                            new SqlParameter("count", focusCount),
                            new SqlParameter("focusUserId", focusUserId)).ToArray();

                    boards.Add(board);
                }
            }

            return boards;
        }
    }
}
