namespace Microsoft.Samples.SocialGames.Worker.Commands
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class GameActionStatisticsCommand : GameActionCommand
    {
        private readonly IStatisticsRepository statisticsRepository;

        public GameActionStatisticsCommand(IStatisticsRepository statisticsRepository)
        {
            this.statisticsRepository = statisticsRepository;
        }

        public override void Do(GameAction gameAction)
        {
            UserStats statistics = this.statisticsRepository.Retrieve(gameAction.UserId);
            if (statistics == null)
            {
                statistics = new UserStats
                {
                    UserId = gameAction.UserId,
                    Victories = 0,
                    Defeats = 0
                };
            }                      

            if (gameAction.Type == GameActionType.Victory)
            {
                statistics.Victories += 1;
            }

            if (gameAction.Type == GameActionType.Defeat)
            {
                statistics.Defeats += 1;
            }

            this.statisticsRepository.Save(statistics);            
        }
   }
}