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

        public ActionResult ServerInterfaceTest()
        {
            return View();
        }

        public ActionResult GameServiceTest()
        {
            return View();
        }

        public ActionResult UserServiceTest()
        {
            return View();
        }
    }
}
