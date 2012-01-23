namespace Microsoft.Samples.SocialGames.Tests.Mocks
{
    using Microsoft.Samples.SocialGames.Web.Services;

    public class StringUserProvider : IUserProvider
    {
        private string userId;

        public StringUserProvider(string userId)
        {
            this.userId = userId;
        }

        public string UserId
        {
            get { return this.userId; }
        }
    }
}
