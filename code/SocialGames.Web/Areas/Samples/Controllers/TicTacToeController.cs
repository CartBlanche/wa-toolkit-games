namespace Microsoft.Samples.SocialGames.Web.Areas.Samples.Controllers
{
    using System.Web.Mvc;
    using Microsoft.Samples.SocialGames.Web.Controllers;

    public class TicTacToeController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult NodeJs()
        {
            this.ViewBag.NodeJsUrl = System.Configuration.ConfigurationManager.AppSettings["NodeJsUrl"];

            return View();
        }
    }
}