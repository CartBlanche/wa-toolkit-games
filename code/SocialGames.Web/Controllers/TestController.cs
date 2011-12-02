namespace Microsoft.Samples.SocialGames.Controllers
{
    using System.Web.Mvc;
    using Microsoft.Samples.SocialGames.Web.Controllers;

    public class TestController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult ServerInterfaceTest()
        {
            return View();
        }

        [Authorize]
        public ActionResult GameServiceTest()
        {
            return View();
        }

        [Authorize]
        public ActionResult UserServiceTest()
        {
            return View();
        }
    }
}
