namespace Microsoft.Samples.SocialGames.Tests
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Web;
    using Microsoft.Samples.SocialGames.GamePlay.Services;
    using Microsoft.Samples.SocialGames.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AuthServiceTest : ServiceTest
    {
        [TestMethod]
        [DeploymentItem(@"Resources\identityProvidersJsonList.txt")]
        public void ShouldReturnLoginSelector()
        {
            var mockContext = new Mock<HttpContextBase>(); 
            var mockRequest = new Mock<HttpRequestBase>();
            var mockHeaders = new NameValueCollection();
            var mockIdentityProviderRepository = new Mock<IIdentityProviderRepository>();

            mockContext.Setup(ctx => ctx.Request).Returns(mockRequest.Object);
            mockRequest.Setup(request => request.ApplicationPath).Returns("/testpath");
            mockRequest.Setup(request => request.Url).Returns(new Uri("http://mysite/"));
            mockRequest.Setup(request => request.Headers).Returns(mockHeaders);

            mockIdentityProviderRepository
                .Setup(rep => rep.GetIdentityProvidersInfoEndpoint("http://mysite/testpath/", "http://mysite/testpath/returnpage"))
                .Returns("identityProvidersJsonList.txt")
                .Verifiable();

            var authService = this.CreateAuthService(mockContext.Object, mockIdentityProviderRepository.Object);

            var selector = authService.LoginSelector("http://mysite/testpath/returnpage");

            var actualIdentityProviders = selector.Content.ReadAsString();
            var expectedIdentityProviders = File.ReadAllText("identityProvidersJsonList.txt");

            Assert.AreEqual<string>(expectedIdentityProviders, actualIdentityProviders);

            mockIdentityProviderRepository.VerifyAll();
        }

        private AuthService CreateAuthService(HttpContextBase context, IIdentityProviderRepository identityProviderRepository)
        {
            return new AuthService(context, identityProviderRepository);
        }
    }
}