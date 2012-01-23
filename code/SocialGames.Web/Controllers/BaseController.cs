namespace Microsoft.Samples.SocialGames.Web.Controllers
{
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.ViewBag.BlobUrl = System.Configuration.ConfigurationManager.AppSettings["BlobUrl"];
            this.ViewBag.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];

            base.OnActionExecuting(filterContext);
        }
    }
}