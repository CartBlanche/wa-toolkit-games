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
                    GameCount = 0,
                    Victories = 0,
                    Defeats = 0
                };
            }

            statistics.GameCount += GetValue(gameAction.CommandData, "GameCount");
            statistics.Defeats += GetValue(gameAction.CommandData, "Defeats");
            statistics.Victories += GetValue(gameAction.CommandData, "Victories");            
            
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