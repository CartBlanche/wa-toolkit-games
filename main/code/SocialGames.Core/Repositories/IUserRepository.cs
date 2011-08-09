namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames.Entities;
    using GameID = System.Guid;
    using UserID = System.String;

    public interface IUserRepository
    {
        void AddOrUpdateUser(UserProfile user);

        UserProfile GetUser(UserID userId);

        void AddOrUpdateUserSession(UserSession userSession);

        string GetUserReference(UserID userId, TimeSpan expiryTime);
    }
}