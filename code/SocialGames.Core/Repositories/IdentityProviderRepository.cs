namespace Microsoft.Samples.SocialGames.Repositories
{
    using System.Configuration;
    using Microsoft.Samples.SocialGames;

    public class IdentityProviderRepository : IIdentityProviderRepository
    {
        public string GetIdentityProvidersInfoEndpoint(string realm, string returnUrl)
        {
            var acsNamespace = ConfigurationManager.AppSettings["AcsNamespace"];
            var identityProvidersInfo = ConfigurationConstants.GetIdentityProvidersInfoEndpoint(acsNamespace, realm, returnUrl ?? string.Empty);

            return identityProvidersInfo;
        }
    }
}