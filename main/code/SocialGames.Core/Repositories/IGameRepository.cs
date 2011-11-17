namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Entities;

    public interface IGameRepository
    {
        void EnsureExist();

        void AddUserToGameQueue(string userId, GameType gameType);

        void AddUserToGameQueue(string userId, Guid gameId);

        void LeaveUserFromGame(string userId, Guid gameId);

        void AddOrUpdateGame(Game game);

        void AddOrUpdateGameQueue(GameQueue gameQueue);

        Game GetGame(Guid gameId);

        GameQueue GetGameQueue(Guid gameQueueId);

        Game StartGame(Guid gameQueueId);

        string GetGameReference(Guid gameId, TimeSpan expiryTime);

        void Invite(string userId, Guid gameQueueId, string message, string url, List<string> users);
    }
}