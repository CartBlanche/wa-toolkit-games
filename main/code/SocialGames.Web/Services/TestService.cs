namespace Microsoft.Samples.SocialGames.GamePlay.Services
{
    using System;
    using System.Collections.Generic;
    using System.Json;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.Tests.GamePlay.Game;

    public class TestService : ServiceBase, ITestService
    {
        private readonly IGameRepository gameRepository;
        private readonly GameActionProcessor gameActionProcessor;

        public TestService()
            : this(new GameRepository(), new HttpContextOrGuestUserProvider(), new ConfiguredGameActionProcessor())
        { 
        }

        public TestService(IGameRepository gameRepository, IUserProvider userProvider, GameActionProcessor gameActionProcessor)
            : base(userProvider)
        {
            this.gameRepository = gameRepository;
            this.gameActionProcessor = gameActionProcessor;
        }

        public HttpResponseMessage Command(Guid gameId, HttpRequestMessage request)
        {
            var formContent = GetFormContent(request);
            var game = this.gameRepository.GetGame(gameId);

            if (game == null)
            {
                game = this.CreateGame(gameId, this.CurrentUserId);
            }

            // Command Type
            int commandType;

            try
            {
                commandType = int.Parse(formContent.type.Value);
            }
            catch
            {
                return BadRequest("Invalid type parameter");                    
            }

            // Command Data
            var jsonCommandData = (JsonObject)(formContent.commandData ?? null);
            IDictionary<string, object> commandData = null;

            if (jsonCommandData != null)
            {
                commandData = jsonCommandData.ToDictionary();
            }

            // Add gameAction
            var gameAction = new GameAction
            {
                Id = Guid.NewGuid(),
                Type = commandType,
                CommandData = commandData,
                UserId = this.CurrentUserId,
                Timestamp = DateTime.UtcNow
            };

            // Cleanup game actions lists
            for (int i = 0; i < game.GameActions.Count(); i++)
            {
                if (game.GameActions[i].Timestamp < DateTime.UtcNow.AddSeconds(-10))
                {
                    game.GameActions.RemoveAt(i);
                    i--;
                }
            }

            game.GameActions.Add(gameAction);
            this.gameRepository.AddOrUpdateGame(game);

            return SuccessResponse;
        }

        private Game CreateGame(Guid gameId, string userId)
        {
            // Create game
            var game = new Game
            {
                Id = gameId,
                Users = new List<GameUser>(),
                ActiveUser = userId,
                Status = GameStatus.Ready,
                Seed = new Random().Next(10000, int.MaxValue)
            };

            game.Users.Add(new GameUser() { UserId = userId, UserName = userId, Weapons = new List<Guid>() });            

            this.gameRepository.AddOrUpdateGame(game);

            return game;
        }
    }
}
