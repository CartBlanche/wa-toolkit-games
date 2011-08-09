namespace Microsoft.Samples.SocialGames.Worker.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.JobEngine;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Helpers;
    using Microsoft.Samples.SocialGames.Repositories;

    public class GameQueueAutoStartCommand : ICommand
    {
        private readonly IGameRepository gameRepository;
        private readonly IWorkerContext workerContext;

        private readonly TimeSpan gameQueueTimeoutWaiting;
        private readonly int maxNumberOfPlayersPerGame;

        public GameQueueAutoStartCommand(IGameRepository gameRepository, IWorkerContext workerContext)
            : this(gameRepository, workerContext, ConfigurationConstants.WaitingForPlayersTimeout, ConfigurationConstants.MaxNumberOfPlayersPerGame)
        {
        }

        public GameQueueAutoStartCommand(IGameRepository gameRepository, IWorkerContext workerContext, TimeSpan gameQueueTimeoutWaiting, int maxNumberOfPlayersPerGame)
        {
            this.gameRepository = gameRepository;
            this.workerContext = workerContext;
            
            this.gameQueueTimeoutWaiting = gameQueueTimeoutWaiting;
            this.maxNumberOfPlayersPerGame = maxNumberOfPlayersPerGame;
        }

        public Guid CurrentGameQueueId
        {
            get
            {
                object gameIdObject;
                return this.workerContext.Context.TryGetValue("currentGameQueueId", out gameIdObject) ? new Guid(gameIdObject.ToString()) : Guid.Empty;
            }

            set
            {
                this.workerContext.Context.TryUpdate("currentGameQueueId", value, this.CurrentGameQueueId);
            }
        }

        public void Do(IDictionary<string, object> context)
        {
            if (this.workerContext.Context.ContainsKey("currentGameQueueId") && this.CurrentGameQueueId != Guid.Empty)
            {
                var currentGameQueue = this.gameRepository.GetGameQueue(this.CurrentGameQueueId);
                
                if (currentGameQueue != null && 
                    currentGameQueue.Users.Count() > 0 && 
                    currentGameQueue.Users.Count() < this.maxNumberOfPlayersPerGame &&
                    currentGameQueue.Status == QueueStatus.Waiting)
                {
                    if (currentGameQueue.TimeElapsed() >= this.gameQueueTimeoutWaiting.TotalSeconds)
                    {
                        if (currentGameQueue.Users.Count() == 1)
                        {
                            currentGameQueue.Status = QueueStatus.Timeout;
                        }
                        else
                        {
                            // Add bots
                            //// (Removed for test and integration)
                            ////while (currentGameQueue.Users.Count() < this.maxNumberOfPlayersPerGame)
                            ////{
                            ////    // TODO: Define special UserIds for bots
                            ////    var userId = ConfigurationConstants.BotUserIdPrefix + Guid.NewGuid();
                            ////    currentGameQueue.Users.Add(
                            ////        new GameUser
                            ////        {
                            ////            UserId = userId,
                            ////            UserName = userId
                            ////        });
                            ////}

                            // Create game
                            var game = new Game
                            {
                                Id = Guid.NewGuid(),
                                Users = currentGameQueue.Users,
                                ActiveUser = currentGameQueue.Users.First().UserId,
                                Status = GameStatus.Waiting,
                                Seed = new Random().Next(10000, int.MaxValue)
                            };

                            currentGameQueue.Status = QueueStatus.Ready;
                            currentGameQueue.GameId = game.Id;
                            
                            this.gameRepository.AddOrUpdateGame(game);
                        }

                        this.gameRepository.AddOrUpdateGameQueue(currentGameQueue);
                    }
                }
            }
        }
    }
}