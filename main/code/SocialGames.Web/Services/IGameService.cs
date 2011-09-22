namespace Microsoft.Samples.SocialGames.GamePlay.Services
{
    using System;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public interface IGameService
    {
        [WebInvoke(Method = "POST", UriTemplate = "queue")]
        HttpResponseMessage Queue(HttpRequestMessage request);

        [WebInvoke(Method = "POST", UriTemplate = "leave/{gameId}")]
        HttpResponseMessage Leave(Guid gameId, HttpRequestMessage request);

        [WebInvoke(Method = "POST", UriTemplate = "create")]
        HttpResponseMessage Create();

        [WebInvoke(Method = "POST", UriTemplate = "join/{gameQueueId}")]
        HttpResponseMessage Join(Guid gameQueueId);

        [WebInvoke(Method = "POST", UriTemplate = "start/{gameQueueId}")]
        HttpResponseMessage Start(Guid gameQueueId);

        [WebInvoke(Method = "POST", UriTemplate = "command/{gameId}")]
        HttpResponseMessage Command(Guid gameId, HttpRequestMessage request);

        [WebInvoke(Method = "POST", UriTemplate = "command/v2/{gameId}")]
        HttpResponseMessage Command2(Guid gameId, HttpRequestMessage request);        
    }
}