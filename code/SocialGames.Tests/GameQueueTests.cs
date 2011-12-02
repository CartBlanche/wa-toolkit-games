namespace Microsoft.Samples.SocialGames.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Common;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Helpers;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.Samples.SocialGames.Tests.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class GameQueueTests
    {
        /* POST /game/queue/ (sessionId) => queueId
         /game/queue/
            ○ Description
                ■ Joins a game queue, where user will have 1 minute to be auto assigned
                to a server generated game; each game will have a maximum of 5
                players. This will be from game type skirmish.
            ○ Method: POST
            ○ Parameters
                ■ sessionId
            ○ Returns
                ■ queueId - Your id in the queue, so we can poll the status of your request.

        GET /game/queue/status (sessionId, queueId) -> status, gameId, timeRemaining, Array of users{id, name}
        /game/queue/status
            ○ Description
                ■ user is in a queue and wants to check the status of their invite
            ○ method: GET
            ○ Parameters
                ■ queueId
                ■ sessionId
            ○ Returns
                ■ status (string)
                    ● ready - Your game is ready
                    ● waiting - Still waiting for all participants
                    ● timeout - Not enough available people to play
                    ● notFound
                ■ gameId - When your added to a game, you’ll get the actual gameId that you’ll be playing.
                ■ timeRemaining - Each request will be 1 minute.
                ■ Array of users.
                    ● id, name

        POST /game/leave/ (sessionID, gameID, reason)
        /game/leave/
            ○ Description
                ■ UserProfile can cancel a game request or may leave a game at any time
            ○ Parameters
                ■ gameId (string)
                ■ reason (string) - Why did the user leave?
                    ● userCancelled - used cancelled on the join game invite page
                    ● userLeft - UserProfile was in game, but decided to leave
                    ● browserClosed - UserProfile closed the browser window
*/

        [TestMethod, Ignore]
        public void TestJoningAndLeavingGameQueues()
        {
            FixedTimeProvider timeProvider = new FixedTimeProvider();
            TimeProvider.Current = timeProvider;
            DateTime startingDateTime = DateTime.Parse("01/01/2001 00:00:00");

            timeProvider.CurrentDateTime = startingDateTime;

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            IGameRepository gameRepository = new GameRepository();

            UserProfile firstUser = new UserProfile() { Id = Guid.NewGuid().ToString(), DisplayName = "John" };
            UserProfile secondUser = new UserProfile() { Id = Guid.NewGuid().ToString(), DisplayName = "Peter" };

            IUserRepository userRepository = new UserRepository();

            userRepository.AddOrUpdateUser(firstUser);
            userRepository.AddOrUpdateUser(secondUser);

            gameRepository.AddUserToSkirmishGameQueue(firstUser);

            GameQueue gameQueue = null;
            //// gameQueue = MagicallyGetGameQueues();
            Assert.AreEqual(gameQueue, new Game { CreationTime = startingDateTime, Id = gameQueue.Id, Users = new List<GameUser> { new GameUser { UserId = firstUser.Id } } });
            Assert.AreEqual(gameQueue.TimeElapsed(), 60);

            timeProvider.CurrentDateTime = startingDateTime.AddSeconds(10);
            Assert.AreEqual(gameQueue.TimeElapsed(), 50);

            gameRepository.AddUserToSkirmishGameQueue(secondUser);
            //// gameQueue = MagicallyGetGameQueues();
            Assert.AreEqual(gameQueue, new Game { CreationTime = startingDateTime, Id = gameQueue.Id, Users = new List<GameUser> { new GameUser { UserId = firstUser.Id }, new GameUser { UserId = secondUser.Id } } });
            Assert.AreEqual(gameQueue.TimeElapsed(), 50);

            timeProvider.CurrentDateTime = startingDateTime.AddSeconds(20);
            Assert.AreEqual(gameQueue, new Game { CreationTime = startingDateTime, Id = gameQueue.Id, Users = new List<GameUser> { new GameUser { UserId = firstUser.Id }, new GameUser { UserId = secondUser.Id } } });
            
            Assert.AreEqual(gameQueue.TimeElapsed(), 40);

            // gameRepository.RemoveUserFromGameQueue(firstUser, gameQueue, "Bored");
            // gameQueue = MagicallyGetGameQueues();
            Assert.AreEqual(gameQueue, new Game { CreationTime = startingDateTime, Id = gameQueue.Id, Users = new List<GameUser> { new GameUser { UserId = secondUser.Id } } });
            Assert.AreEqual(gameQueue.TimeElapsed(), 40);

            timeProvider.CurrentDateTime = startingDateTime.AddSeconds(30);
            Assert.AreEqual(gameQueue.TimeElapsed(), 30);

            // gameRepository.RemoveUserFromGameQueue(secondUser, gameQueue, "Also Bored");
            // gameQueue = MagicallyGetGameQueues();
            Assert.AreEqual(gameQueue, new Game { CreationTime = startingDateTime, Id = gameQueue.Id, Users = new List<GameUser>() });
        }
    }
}