namespace Microsoft.Samples.SocialGames.Web.Services
{
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public interface IEventService
    {
        [WebInvoke(Method = "POST", UriTemplate = "post/{topic}")]
        HttpResponseMessage PostEvent(string topic, HttpRequestMessage request);
    }
}