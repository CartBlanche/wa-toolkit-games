namespace Microsoft.Samples.SocialGames.Worker.Commands
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Common.JobEngine;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class LeaveGameCommand : ICommand
    {
        private readonly IUserRepository userRepository;
        private readonly IGameRepository gameRepository;

        public LeaveGameCommand(IUserRepository userRepository, IGameRepository gameRepository)
        {
            this.userRepository = userRepository;
            this.gameRepository = gameRepository;
        }

        public void Do(IDictionary<string, object> context)
        {
            var userId = context["userId"].ToString();
            var gameId = new Guid(context["gameId"].ToString());

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userSession = new UserSession
                {
                    UserId = userId,
                    ActiveGameQueueId = Guid.Empty
                };

                this.userRepository.AddOrUpdateUserSession(userSession);
            }

            var game = this.gameRepository.GetGame(gameId);

            if (game != null)
            {
                if (game.Users.RemoveAll(u => u.UserId == userId) > 0)
                {
                    this.gameRepository.AddOrUpdateGame(game);
                }
            }
            else
            {
                // gameId parameter is the gameQueueId
                var gameQueue = this.gameRepository.GetGameQueue(gameId);

                if (gameQueue != null)
                {
                    if (gameQueue.Users.RemoveAll(u => u.UserId == userId) > 0)
                    {
                        this.gameRepository.AddOrUpdateGameQueue(gameQueue);
                    }
                }
            }
        }
    }
}