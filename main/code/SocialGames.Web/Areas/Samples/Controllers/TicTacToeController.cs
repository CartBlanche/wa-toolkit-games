namespace Microsoft.Samples.SocialGames.Web.Areas.Samples.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Samples.SocialGames.Web.Controllers;

    public class TicTacToeController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}