namespace Microsoft.Samples.Tankster.GamePlay.Services
{
    using System;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.Samples.Tankster.Entities;

    [ServiceContract]
    public interface IEventService
    {
        [WebInvoke(Method = "POST", UriTemplate = "post/{topic}")]
        HttpResponseMessage PostEvent(string topic, HttpRequestMessage request);
    }
}