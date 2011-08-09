namespace Microsoft.Samples.SocialGames.GamePlay.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.Samples.SocialGames;

    public class HttpContextOrGuestUserProvider : IUserProvider
    {
        private string guestUserId;

        public HttpContextOrGuestUserProvider()
            : this(ConfigurationConstants.GuestUserid)
        {
        }

        public HttpContextOrGuestUserProvider(string guestUserId)
        {
            this.guestUserId = guestUserId;
        }

        public string UserId
        {
            get
            {
                var wrapper = new HttpContextWrapper(HttpContext.Current);
                return
                    wrapper.User.Identity.IsAuthenticated ?
                    ((IClaimsIdentity)wrapper.User.Identity).Claims.Single(c => c.ClaimType == ClaimTypes.NameIdentifier).Value :
                    this.guestUserId;
            }
        }
    }
}