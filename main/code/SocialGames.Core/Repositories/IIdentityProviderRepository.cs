namespace Microsoft.Samples.SocialGames.Repositories
{
    public interface IIdentityProviderRepository
    {
        string GetIdentityProvidersInfoEndpoint(string realm, string returnUrl);
    }
}