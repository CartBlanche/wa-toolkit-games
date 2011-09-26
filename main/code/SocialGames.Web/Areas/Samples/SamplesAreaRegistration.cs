namespace Microsoft.Samples.SocialGames.Web.Areas.Samples
{
    using System.Web.Mvc;

    public class SamplesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Samples";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Samples_default",
                "Samples/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}
