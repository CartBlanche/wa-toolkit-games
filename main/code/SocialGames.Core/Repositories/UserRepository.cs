namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure;

    public class UserRepository : IUserRepository
    {
        private readonly IAzureBlobContainer<UserProfile> userContainer;
        private readonly IAzureBlobContainer<UserSession> userSessionContainer;
        private readonly IAzureBlobContainer<Friends> friendsContainer;

        public UserRepository()
            : this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"))
        {
        }

        public UserRepository(CloudStorageAccount account)
            : this(account, ConfigurationConstants.UsersContainerName, ConfigurationConstants.UserSessionsContainerName, ConfigurationConstants.FriendsContainerName)
        {
        }

        public UserRepository(CloudStorageAccount account, string usersContainerName, string userSessionContainerName, string friendsContainerName)
            : this(new AzureBlobContainer<UserProfile>(account, usersContainerName, true), new AzureBlobContainer<UserSession>(account, userSessionContainerName, true), new AzureBlobContainer<Friends>(account, friendsContainerName, true))
        {
        }

        public UserRepository(IAzureBlobContainer<UserProfile> userContainer, IAzureBlobContainer<UserSession> userSessionContainer, IAzureBlobContainer<Friends> friendsContainer)
        {
            if (userContainer == null)
            {
                throw new ArgumentNullException("userContainer");
            }

            if (userSessionContainer == null)
            {
                throw new ArgumentNullException("userSessionContainer");
            }

            if (friendsContainer == null)
            {
                throw new ArgumentNullException("friendsContainer");
            }

            this.userContainer = userContainer;
            this.userContainer.EnsureExist(true);

            this.userSessionContainer = userSessionContainer;
            this.userSessionContainer.EnsureExist(true);

            this.friendsContainer = friendsContainer;
            this.friendsContainer.EnsureExist(true);
        }

        public void AddOrUpdateUser(UserProfile user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.Id == string.Empty)
            {
                throw new ArgumentException("User Id cannot be empty");
            }

            this.userContainer.Save(user.Id.ToString(), user);
        }

        public UserProfile GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User Id cannot be empty nor null");
            }

            return this.userContainer.Get(userId.ToString());
        }

        public void AddOrUpdateUserSession(UserSession userSession)
        {
            if (string.IsNullOrWhiteSpace(userSession.UserId))
            {
                throw new ArgumentException("User Id cannot be empty");
            }

            this.userSessionContainer.Save(userSession.UserId, userSession);
        }

        public string GetUserReference(string userId, TimeSpan expiryTime)
        {
            return this.userContainer.GetSharedAccessSignature(userId.ToString(), DateTime.UtcNow.Add(expiryTime));
        }

        public void AddFriend(string userId, string friendUserId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User Id cannot be empty nor null");
            }

            if (string.IsNullOrEmpty(friendUserId))
            {
                throw new ArgumentException("FriendUser Id cannot be empty nor null");
            }

            Friends friends = this.friendsContainer.Get(userId);

            if (friends == null)
            {
                friends = new Friends() { Id = userId };
            }

            if (friends.Users.Contains(friendUserId))
            {
                return;
            }

            friends.Users.Add(friendUserId);

            this.friendsContainer.Save(userId, friends);
        }

        public List<string> GetFriends(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User Id cannot be empty nor null");
            }

            Friends friends = this.friendsContainer.Get(userId);

            if (friends == null)
            {
                return new List<string>();
            }

            return friends.Users;
        }

        public List<UserInfo> GetFriendsInfo(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User Id cannot be empty nor null");
            }

            Friends friends = this.friendsContainer.Get(userId);
            List<UserInfo> list = new List<UserInfo>();

            if (friends == null)
            {
                return list;
            }

            foreach (string friend in friends.Users)
            {
                var profile = this.GetUser(friend);
                var info = new UserInfo() { Id = friend };
                if (profile == null || string.IsNullOrEmpty(profile.DisplayName))
                {
                    info.DisplayName = friend;
                }
                else
                {
                    info.DisplayName = profile.DisplayName;
                }

                list.Add(info);
            }

            return list;
        }
    }
}