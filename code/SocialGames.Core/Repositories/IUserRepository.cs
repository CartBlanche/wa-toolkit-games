namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Entities;
    using GameID = System.Guid;
    using UserID = System.String;

    public interface IUserRepository
    {
        void AddOrUpdateUser(UserProfile user);

        UserProfile GetUser(UserID userId);

        void AddOrUpdateUserSession(UserSession userSession);

        string GetUserReference(UserID userId, TimeSpan expiryTime);

        List<UserID> GetFriends(UserID userId);

        List<UserInfo> GetFriendsInfo(UserID userId);

        void AddFriend(UserID userId, UserID friendUserId);
    }
}
