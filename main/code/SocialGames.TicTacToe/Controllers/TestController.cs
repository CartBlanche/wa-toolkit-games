using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.Samples.SocialGames.TicTacToe.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Game()
        {
            return View();
        }

        public ActionResult ServerInterfaceTest()
        {
            return View();
        }
    }
}
