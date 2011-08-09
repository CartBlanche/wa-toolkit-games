namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.WindowsAzure;

    public class UserRepository : IUserRepository
    {
        private readonly IAzureBlobContainer<UserProfile> userContainer;
        private readonly IAzureBlobContainer<UserSession> userSessionContainer;

        public UserRepository()
            : this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString"))
        { 
        }

        public UserRepository(CloudStorageAccount account)
            : this(account, ConfigurationConstants.UsersContainerName, ConfigurationConstants.UserSessionsContainerName)
        { 
        }

        public UserRepository(CloudStorageAccount account, string usersContainerName, string userSessionContainerName)
            : this(new AzureBlobContainer<UserProfile>(account, usersContainerName, true), new AzureBlobContainer<UserSession>(account, userSessionContainerName, true))
        {
        }

        public UserRepository(IAzureBlobContainer<UserProfile> userContainer, IAzureBlobContainer<UserSession> userSessionContainer)
        {
            if (userContainer == null)
            {
                throw new ArgumentNullException("userContainer");
            }

            if (userSessionContainer == null)
            {
                throw new ArgumentNullException("userSessionContainer");
            }

            this.userContainer = userContainer;
            this.userContainer.EnsureExist(true);

            this.userSessionContainer = userSessionContainer;
            this.userSessionContainer.EnsureExist(true);
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
    }
}