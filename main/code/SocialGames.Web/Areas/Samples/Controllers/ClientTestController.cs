namespace Microsoft.Samples.SocialGames.Web.Areas.Samples.Controllers
{
    using System.Web.Mvc;

    public class ClientTestController : Controller
    {
        public ActionResult TicTacToeGameTest()
        {
            return View();
        }

        public ActionResult TicTacToeBoardTest()
        {
            return View();
        }

        public ActionResult FourInARowGameTest()
        {
            return View();
        }

        public ActionResult FourInARowBoardTest()
        {
            return View();
        }
    }
}
