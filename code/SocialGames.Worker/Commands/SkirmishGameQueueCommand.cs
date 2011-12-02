namespace Microsoft.Samples.SocialGames.Worker.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.JobEngine;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class SkirmishGameQueueCommand : ICommand
    {
        private readonly IUserRepository userRepository;
        private readonly IGameRepository gameRepository;
        private readonly IWorkerContext workerContext;

        private readonly int maxNumberOfPlayersPerGame;
        private readonly TimeSpan gameQueueTimeoutWaiting;

        public SkirmishGameQueueCommand(IUserRepository userRepository, IGameRepository gameRepository, IWorkerContext workerContext)
            : this(userRepository, gameRepository, workerContext, ConfigurationConstants.MaxNumberOfPlayersPerGame, ConfigurationConstants.WaitingForPlayersTimeout)
        { 
        }

        public SkirmishGameQueueCommand(IUserRepository userRepository, IGameRepository gameRepository, IWorkerContext workerContext, int maxNumberOfPlayersPerGame, TimeSpan gameQueueTimeoutWaiting)
        {
            this.userRepository = userRepository;
            this.gameRepository = gameRepository;
            this.workerContext = workerContext;

            // Ensure currentGame
            this.workerContext.Context.GetOrAdd("currentGameQueueId", Guid.Empty);

            this.maxNumberOfPlayersPerGame = maxNumberOfPlayersPerGame;
            this.gameQueueTimeoutWaiting = gameQueueTimeoutWaiting;
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
            var userId = context["userId"].ToString();

            if (userId != string.Empty)
            {
                var currentGameQueue = this.gameRepository.GetGameQueue(this.CurrentGameQueueId);
                var user = this.userRepository.GetUser(userId);
                var gameUser = new GameUser
                {
                    UserId = user.Id,
                    UserName = user.DisplayName
                };

                // Create/Update gameQueue blob
                if (currentGameQueue == null || currentGameQueue.Status != QueueStatus.Waiting)
                {
                    this.CurrentGameQueueId = Guid.NewGuid();
                    
                    // Create game queue
                    currentGameQueue = new GameQueue
                    {
                        Id = this.CurrentGameQueueId,
                        Users = new List<GameUser> { gameUser },
                        Status = QueueStatus.Waiting, 
                        GameId = Guid.Empty
                    };
                }
                else if (!currentGameQueue.Users.Any(u => u.UserId == gameUser.UserId))
                {
                    currentGameQueue.Users.Add(gameUser);
                }

                if (currentGameQueue.Users.Count >= this.maxNumberOfPlayersPerGame)
                {
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

                var userSession = new UserSession
                {
                    UserId = userId,
                    ActiveGameQueueId = currentGameQueue.Id
                };

                this.gameRepository.AddOrUpdateGameQueue(currentGameQueue);
                this.userRepository.AddOrUpdateUserSession(userSession);
            }
        }
    }
}