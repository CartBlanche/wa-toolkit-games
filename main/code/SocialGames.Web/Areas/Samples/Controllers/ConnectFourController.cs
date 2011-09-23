namespace Microsoft.Samples.SocialGames.Web.Areas.Samples.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class ConnectFourController : Controller
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

        public ActionResult NodeJsEx()
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

        private void SetConfigurationData()
        {
            this.ViewBag.BlobUrl = System.Configuration.ConfigurationManager.AppSettings["BlobUrl"];
            this.ViewBag.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
            this.ViewBag.NodeJsUrl = System.Configuration.ConfigurationManager.AppSettings["NodeJsUrl"];
        }
    }
}