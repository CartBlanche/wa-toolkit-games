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
            if (string.IsNullOrWhiteSpace(gameAction.UserId))
            {
                return;
            }

            UserStats statistics = this.statisticsRepository.Retrieve(gameAction.UserId);
            if (statistics == null)
            {
                statistics = new UserStats
                {
                    UserId = gameAction.UserId,                    
                    Victories = 0,
                    Defeats = 0,
                    GameCount = 0
                };
            }
            
            statistics.Defeats += GetValue(gameAction.CommandData, "Defeats");
            statistics.Victories += GetValue(gameAction.CommandData, "Victories");
            statistics.GameCount += GetValue(gameAction.CommandData, "GameCount");
            
            this.statisticsRepository.Save(statistics);            
        }

        private static int GetValue(IDictionary<string, object> commandData, string key)
        {
            var value = 0;

            if (commandData.ContainsKey(key))
            {
                int.TryParse(commandData[key].ToString(), out value);
            }

            return value;
        }
   }
}