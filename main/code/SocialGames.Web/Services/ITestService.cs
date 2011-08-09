namespace Microsoft.Samples.SocialGames.GamePlay.Services
{
    using System;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public interface ITestService
    {
        [WebInvoke(Method = "POST", UriTemplate = "command/{gameId}")]
        HttpResponseMessage Command(Guid gameId, HttpRequestMessage request);
    }
}