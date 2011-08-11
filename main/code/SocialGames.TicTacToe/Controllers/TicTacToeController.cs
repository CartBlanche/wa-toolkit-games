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
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
    }
}
