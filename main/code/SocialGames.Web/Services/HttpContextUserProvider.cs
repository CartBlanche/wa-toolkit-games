namespace Microsoft.Samples.Tankster.GamePlay.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.IdentityModel.Claims;

    public class HttpContextUserProvider : IUserProvider
    {
        public string UserId
        {
            get
            {
                var wrapper = new HttpContextWrapper(HttpContext.Current);
                return
                    wrapper.User.Identity.IsAuthenticated ?
                    ((IClaimsIdentity)wrapper.User.Identity).Claims.Single(c => c.ClaimType == ClaimTypes.NameIdentifier).Value :
                    string.Empty;
            }
        }
    }
}