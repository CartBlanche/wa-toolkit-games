namespace Microsoft.Samples.SocialGames.GamePlay.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Web.Mvc;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.IdentityModel.Protocols.WSFederation;
    using Microsoft.Samples.SocialGames;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public class AccountController : Controller
    {
        private readonly IUserRepository userRepository;

        public AccountController()
            : this(new UserRepository())
        {
        }

        public AccountController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpPost]
        public ActionResult LogOn()
        {
            if (this.Request.IsAuthenticated)
            {
                // Ensure user profile
                var userId = this.GetClaimValue(ClaimTypes.NameIdentifier);
                var userProfile = this.userRepository.GetUser(userId);
                if (userProfile == null)
                {
                    var loginType = this.GetClaimValue(ConfigurationConstants.IdentityProviderClaimType);
                    userProfile = new UserProfile
                    {
                        Id = userId,
                        DisplayName = Thread.CurrentPrincipal.Identity.Name ?? string.Empty,
                        LoginType = loginType,
                        AssociatedUserAccount = 
                            loginType.StartsWith("Facebook", StringComparison.OrdinalIgnoreCase) ? 
                                this.GetClaimValue(ConfigurationConstants.FacebookAccessTokenClaimType) :
                                string.Empty
                    };

                    this.userRepository.AddOrUpdateUser(userProfile);
                }
                
                var effectiveReturnUrl = this.GetContextFromRequest();

                if (!string.IsNullOrWhiteSpace(effectiveReturnUrl))
                {
                    return Redirect(effectiveReturnUrl);
                }
            }

            return Redirect("/Client/WarRoom");
        }

        public ActionResult Friends()
        {
            return View();
        }

        public ActionResult Profile()
        {
            return View();
        }

        private string GetContextFromRequest()
        {
            Uri requestBaseUrl = WSFederationMessage.GetBaseUrl(this.Request.Url);
            var message = WSFederationMessage.CreateFromNameValueCollection(requestBaseUrl, this.Request.Form);
            
            return message != null ? message.Context : string.Empty;
        }

        private string GetClaimValue(string claimType)
        {
            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated");
            }

            var claimsIdentity = (IClaimsIdentity)Thread.CurrentPrincipal.Identity;

            if (!claimsIdentity.Claims.Any(c => c.ClaimType.Equals(claimType, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Claim not found: " + claimType);
            }

            return claimsIdentity.Claims.FirstOrDefault(c => c.ClaimType.Equals(claimType, StringComparison.OrdinalIgnoreCase)).Value;
        }
    }
}