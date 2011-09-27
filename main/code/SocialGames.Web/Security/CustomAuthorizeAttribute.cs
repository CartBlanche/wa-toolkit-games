namespace Microsoft.Samples.SocialGames.Web.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.Headers["player"] != null)
            {
                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
}