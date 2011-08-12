namespace Microsoft.Samples.SocialGames.TicTacToe.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class TestController : Controller
    {
        public ActionResult Index()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult Game()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult ServerInterfaceTest()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult GameServiceTest()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult UserServiceTest()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult TicTacToeGameTest()
        {
            return View();
        }

        public ActionResult CanvasTest()
        {
            return View();
        }

        private void SetConfigurationData()
        {
            this.ViewBag.BlobUrl = System.Configuration.ConfigurationManager.AppSettings["BlobUrl"];
            this.ViewBag.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
        }
    }
}
