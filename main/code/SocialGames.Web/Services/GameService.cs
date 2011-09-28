namespace Microsoft.Samples.SocialGames.GamePlay.Services
{
    using System;
    using System.Collections.Generic;
    using System.Json;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.Serialization.Json;
    using System.Web.Mvc;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.GamePlay.Extensions;
    using Microsoft.Samples.SocialGames.Repositories;

    public class GameService : ServiceBase, IGameService
    {
        private readonly IGameRepository gameRepository;
        private readonly IUserRepository userRepository;

        public GameService()
            : this(new GameRepository(), new UserRepository(), new HttpContextUserProvider())
        { 
        }

        public GameService(IGameRepository gameRepository, IUserRepository userRepository, IUserProvider userProvider)
            : base(userProvider)
        {            
            this.gameRepository = gameRepository;
            this.userRepository = userRepository;
        }

        public HttpResponseMessage Queue(HttpRequestMessage request)
        {
            var formContent = GetFormContent(request);
            GameType gameType;

            if (!Enum.TryParse<GameType>(formContent.gameType.Value, true, out gameType))
            {
                return BadRequest("Invalid gameType parameter");
            }

            try
            {
                // Update userSession blob
                var userSession = new UserSession
                {
                    UserId = CurrentUserId,
                    ActiveGameQueueId = Guid.Empty
                };

                this.userRepository.AddOrUpdateUserSession(userSession);
                this.gameRepository.AddUserToGameQueue(CurrentUserId, gameType);                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return SuccessResponse;
        }

        public HttpResponseMessage Leave(Guid gameId, HttpRequestMessage request)
        {
            // TODO: what about reason parameter?
            ////var formContent = GetFormContent(request);
            ////var reason = formContent.reason;

            try
            {
                this.gameRepository.LeaveUserFromGame(CurrentUserId, gameId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return SuccessResponse;
        }

        public HttpResponseMessage Create()
        {
            Guid gameQueueId = Guid.NewGuid();
            string userId = this.CurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User does not exist. User Id: " + userId);
            }

            UserProfile profile = this.userRepository.GetUser(CurrentUserId);

            if (profile == null)
            {
                return BadRequest("User does not exist. User Id: " + userId);
            }

            GameUser user = new GameUser()
            {
                UserId = profile.Id,
                UserName = profile.DisplayName,
                Weapons = new List<Guid>()
            };

            GameQueue gameQueue = new GameQueue() 
            {
                Id = gameQueueId,
                CreationTime = DateTime.UtcNow,
                Status = QueueStatus.Waiting,
                Users = new List<GameUser>() { user }
            };

            this.gameRepository.AddOrUpdateGameQueue(gameQueue);
            return HttpResponse<string>(gameQueueId.ToString());
        }

        public HttpResponseMessage Join(Guid gameQueueId)
        {
            try
            {
                this.gameRepository.AddUserToGameQueue(CurrentUserId, gameQueueId);

                var queue = this.gameRepository.GetGameQueue(gameQueueId);
                var currentUserId = this.CurrentUserId;

                foreach (var user in queue.Users)
                {
                    this.userRepository.AddFriend(currentUserId, user.UserId);
                    this.userRepository.AddFriend(user.UserId, currentUserId);
                }

                return SuccessResponse;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public HttpResponseMessage Start(Guid gameQueueId)
        {
            try
            {
                this.gameRepository.StartGame(gameQueueId);

                return SuccessResponse;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public HttpResponseMessage Command(Guid gameId, HttpRequestMessage request)
        {
            var formContent = GetFormContent(request);
            var game = this.gameRepository.GetGame(gameId);
            if (game == null)
            {
                return BadRequest("Game does not exist. Game Id: " + gameId);
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

            try
            {
                // Add gameAction
                var gameAction = new GameAction
                {
                    Id = Guid.NewGuid(),
                    Type = commandType,
                    CommandData = commandData,
                    UserId = this.CurrentUserId,
                    Timestamp = DateTime.UtcNow
                };

                game.GameActions.Add(gameAction);
                this.gameRepository.AddOrUpdateGame(game);

                return SuccessResponse;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        public HttpResponseMessage Invite(Guid gameQueueId, HttpRequestMessage request)
        {
            var formContent = GetFormContent(request);
            var users = formContent.users != null ?
                    ((JsonArray)formContent.users).ToObjectArray().Select(o => o.ToString()).ToList() :
                    null;

            this.gameRepository.Invite(this.CurrentUserId, gameQueueId, formContent.message, formContent.url, users);

            return SuccessResponse;
        }

        private string GetCommandDataValue(IDictionary<string, object> commandData, string commandDataKey)
        {
            if (!commandData.ContainsKey(commandDataKey))
            {
                throw new InvalidOperationException(commandDataKey + " parameter cannot be null or empty");
            }

            return commandData[commandDataKey].ToString();
        }
    }
}
