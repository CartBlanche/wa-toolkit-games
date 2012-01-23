namespace Microsoft.Samples.SocialGames.Web.Areas.Samples.Controllers
{
    using System.Web.Mvc;
    using Microsoft.Samples.SocialGames.Web.Controllers;

    public class FourInARowController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}