namespace Microsoft.Samples.SocialGames.Repositories
{
    using System.Data.Entity;
    using Microsoft.Samples.SocialGames.Entities;

    public class StatisticsContext : DbContext
    {
        public StatisticsContext()
            : base()
        {
        }

        public StatisticsContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<UserStats> Statistics 
        { 
            get; 
            set;
        }
    }
}
