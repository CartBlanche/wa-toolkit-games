namespace Microsoft.Samples.SocialGames.Tests
{
    using System.Web;
    using Moq;

    public abstract class ServiceTest
    {
        protected Mock<HttpContextBase> CreateHttpContext(string username)
        {
            var httpContextBase = new Mock<HttpContextBase>();
            httpContextBase.Setup(obj => obj.User.Identity.Name)
                           .Returns(username)
                           .Verifiable();
            
            return httpContextBase;
        }
    }
}