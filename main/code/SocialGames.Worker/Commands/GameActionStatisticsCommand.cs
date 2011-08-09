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
                    XP = 0
                };
            }                      

            statistics.Accuracy = this.UpdateAverage(gameAction.CommandData, "accuracy", statistics.Accuracy);
            statistics.TerrainDeformation = this.UpdateAverage(gameAction.CommandData, "terrainDeformation", statistics.TerrainDeformation);
            statistics.Kills = this.UpdateAverage(gameAction.CommandData, "kills", statistics.Kills);            
            statistics.XP = this.UpdateNet(gameAction.CommandData, "xp", statistics.XP);

            if (gameAction.Type == GameActionType.EndGame)
            {
                statistics.Victories += 1;
            }

            this.UpdateRank(statistics);

            this.statisticsRepository.Save(statistics);            
        }

        private int UpdateAverage(IDictionary<string, object> commandData, string score, int storedValue)
        {
            if (!commandData.ContainsKey(score) || string.IsNullOrEmpty(score))
            {
                return storedValue;
            }
           
            int newValue;
            int.TryParse(commandData[score].ToString(), out newValue);

            if (storedValue == 0)
            {
                return newValue;
            }

            return (storedValue + newValue) / 2;          
        }

        private float UpdateAverage(IDictionary<string, object> commandData, string score, float storedValue)
        {
            if (!commandData.ContainsKey(score) || string.IsNullOrEmpty(score))
            {
                return storedValue;
            }

            float newValue;
            float.TryParse(commandData[score].ToString(), out newValue);

            if (storedValue == 0)
            {
                return newValue;
            }

            return (storedValue + newValue) / 2;
        }

        private float UpdateNet(IDictionary<string, object> commandData, string score, float storedValue)
        {
            if (!commandData.ContainsKey(score) || string.IsNullOrEmpty(score))
            {
                return storedValue;
            }
           
            int newValue;
            int.TryParse(commandData[score].ToString(), out newValue);
            return storedValue + newValue;
        }

        private void UpdateRank(UserStats statistics) 
        {
            if (statistics.XP > 0)
            {
                statistics.Rank = (float)Math.Floor(Math.Sqrt(statistics.XP / 250));
            }
        }
    }
}