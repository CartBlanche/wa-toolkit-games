namespace Microsoft.Samples.SocialGames.TicTacToe.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class TicTacToeController : Controller
    {
        public ActionResult Index()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult IndexEx()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult Login()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult OldBrowser()
        {
            return View();
        }

        public ActionResult NodeJs()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult NodeJsEx()
        {
            this.SetConfigurationData();
            return View();
        }

        private void SetConfigurationData()
        {
            this.ViewBag.BlobUrl = System.Configuration.ConfigurationManager.AppSettings["BlobUrl"];
            this.ViewBag.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
            this.ViewBag.NodeJsUrl = System.Configuration.ConfigurationManager.AppSettings["NodeJsUrl"];
        }
    }
}