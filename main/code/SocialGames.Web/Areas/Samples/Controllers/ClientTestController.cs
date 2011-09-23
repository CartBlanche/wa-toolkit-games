namespace Microsoft.Samples.SocialGames.Web.Areas.Samples.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class ClientTestController : Controller
    {
        public ActionResult Game()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult TicTacToeGameTest()
        {
            return View();
        }

        public ActionResult TicTacToeBoardTest()
        {
            return View();
        }

        public ActionResult ConnectFourGameTest()
        {
            return View();
        }

        public ActionResult ConnectFourBoardTest()
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
