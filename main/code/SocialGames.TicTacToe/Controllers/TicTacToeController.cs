using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Samples.SocialGames.TicTacToe.Controllers
{
    public class TicTacToeController : Controller
    {
        //
        // GET: /TicTacToe/

        public ActionResult Index()
        {
            this.SetConfigurationData();
            return View();
        }

        public ActionResult Login()
        {
            this.SetConfigurationData();
            return View();
        }

        private void SetConfigurationData()
        {
            this.ViewBag.BlobUrl = System.Configuration.ConfigurationManager.AppSettings["BlobUrl"];
            this.ViewBag.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
        }
    }
}

