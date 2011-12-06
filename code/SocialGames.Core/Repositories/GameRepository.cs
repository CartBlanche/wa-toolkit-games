namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure;
    using Microsoft.Samples.SocialGames.Common;

    public class GameRepository : IGameRepository, IInitializer
    {
        private readonly IAzureQueue<SkirmishGameQueueMessage> skirmishGameQueue;
        private readonly IAzureQueue<LeaveGameMessage> leaveGameQueue;
        private readonly IAzureBlobContainer<Game> gameContainer;
        private readonly IAzureBlobContainer<GameQueue> gameQueueContainer;
        private readonly IAzureBlobContainer<UserProfile> userContainer;
        private readonly IAzureQueue<InviteMessage> inviteQueue;

        public GameRepository(IAzureBlobContainer<Game> gameContainer, IAzureBlobContainer<GameQueue> gameQueueContainer, IAzureQueue<SkirmishGameQueueMessage> skirmishGameMessageQueue, IAzureQueue<LeaveGameMessage> leaveGameMessageQueue, IAzureBlobContainer<UserProfile> userContainer, IAzureQueue<InviteMessage> inviteQueue)
        {
            if (gameContainer == null)
            {
                throw new ArgumentNullException("gameContainer");
            }

            if (gameQueueContainer == null)
            {
                throw new ArgumentNullException("gameQueueContainer");
            }

            if (skirmishGameMessageQueue == null)
            {
                throw new ArgumentNullException("skirmishGameMessageQueue");
            }

            if (leaveGameMessageQueue == null)
            {
                throw new ArgumentNullException("leaveGameMessageQueue");
            }

            if (userContainer == null)
            {
                throw new ArgumentNullException("userContainer");
            }

            if (inviteQueue == null)
            {
                throw new ArgumentNullException("inviteQueue");
            }

            this.skirmishGameQueue = skirmishGameMessageQueue;
            this.leaveGameQueue = leaveGameMessageQueue;
            this.gameContainer = gameContainer;
            this.gameQueueContainer = gameQueueContainer;
            this.userContainer = userContainer;
            this.inviteQueue = inviteQueue;
        }

        public void Initialize()
        {
            this.skirmishGameQueue.EnsureExist();
            this.leaveGameQueue.EnsureExist();
            this.gameContainer.EnsureExist(true);
            this.gameQueueContainer.EnsureExist();
            this.userContainer.EnsureExist(true);
            this.inviteQueue.EnsureExist();
        }

        public void AddUserToGameQueue(string userId, GameType gameType)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            if (gameType == GameType.Skirmish)
            {
                var skirmishMessage = new SkirmishGameQueueMessage { UserId = userId };

                this.skirmishGameQueue.AddMessage(skirmishMessage);
                return;
            }
        }

        public void AddUserToGameQueue(string userId, Guid gameQueueId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId");
            }

            if (gameQueueId == null || gameQueueId == Guid.Empty)
            {
                throw new ArgumentException("Game Queue Id cannot be null or empty");
            }

            var currentGameQueue = this.gameQueueContainer.Get(gameQueueId.ToString());

            if (currentGameQueue == null)
            {
                throw new InvalidOperationException(string.Format("Game Queue does not exist: {0}", gameQueueId));
            }

            var user = this.userContainer.Get(userId);
            var gameUser = new GameUser
            {
                UserId = user.Id,
                UserName = user.DisplayName
            };

            if (!currentGameQueue.Users.Any(u => u.UserId == gameUser.UserId))
            {
                currentGameQueue.Users.Add(gameUser);
            }

            this.gameQueueContainer.Save(gameQueueId.ToString(), currentGameQueue);
        }

        public void LeaveUserFromGame(string userId, Guid gameId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User Id cannot be null or empty");
            }

            if (gameId == null || gameId == Guid.Empty)
            {
                throw new ArgumentException("Game Id cannot be null or empty");
            }

            var message = new LeaveGameMessage
            {
                UserId = userId,
                GameId = gameId
            };

            this.leaveGameQueue.AddMessage(message);
        }

        public void AddOrUpdateGame(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            if (game.Id == Guid.Empty)
            {
                throw new ArgumentException("Game Id cannot be empty");
            }

            this.gameContainer.Save(game.Id.ToString(), game);
        }

        public Game GetGame(Guid gameId)
        {
            return this.gameContainer.Get(gameId.ToString());
        }

        public string GetGameReference(Guid gameId, TimeSpan expiryTime)
        {
            return this.gameContainer.GetSharedAccessSignature(gameId.ToString(), DateTime.UtcNow.Add(expiryTime));
        }

        public void AddOrUpdateGameQueue(GameQueue gameQueue)
        {
            if (gameQueue == null)
            {
                throw new ArgumentNullException("gameQueue");
            }

            if (gameQueue.Id == Guid.Empty)
            {
                throw new ArgumentException("GameQueue Id cannot be empty");
            }

            this.gameQueueContainer.Save(gameQueue.Id.ToString(), gameQueue);
        }

        public GameQueue GetGameQueue(Guid gameQueueId)
        {
            return this.gameQueueContainer.Get(gameQueueId.ToString());
        }

        public Game StartGame(Guid gameQueueId)
        {
            GameQueue queue = this.gameQueueContainer.Get(gameQueueId.ToString());

            if (queue == null)
            {
                throw new InvalidOperationException(string.Format("Game Queue does not exist: {0}", gameQueueId));
            }

            if (queue.Status != QueueStatus.Waiting)
            {
                throw new InvalidOperationException(string.Format("Game Queue Status is not Waiting: {0}", gameQueueId));
            }

            // Create game
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Users = queue.Users,
                ActiveUser = queue.Users.First().UserId,
                Status = GameStatus.Waiting,
                Seed = new Random().Next(10000, int.MaxValue)
            };

            queue.Status = QueueStatus.Ready;
            queue.GameId = game.Id;

            this.gameContainer.Save(game.Id.ToString(), game);
            this.gameQueueContainer.Save(gameQueueId.ToString(), queue);

            return game;
        }

        public void Invite(string userId, Guid gameQueueId, string msg, string url, List<string> users)
        {
            DateTime timestamp = DateTime.UtcNow;

            foreach (var user in users)
            {
                InviteMessage message = new InviteMessage()
                {
                    UserId = userId,
                    GameQueueId = gameQueueId,
                    InvitedUserId = user,
                    Message = msg,
                    Url = url,
                    Timestamp = timestamp
                };

                this.inviteQueue.AddMessage(message);
            }
        }
    }
}