namespace Microsoft.Samples.SocialGames.Web.Services
{
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public interface IAuthService
    {
        [WebGet(UriTemplate = "loginSelector?returnUrl={returnUrl}")]
        HttpResponseMessage LoginSelector(string returnUrl);
    }
}