namespace Microsoft.Samples.SocialGames.Web.Areas.Samples.Extensions
{
    using System.Web.Mvc;

    public static class UrlHelperExtensions
    {
        public static string AreaContent(this UrlHelper urlHelper, string contentPath)
        {
            var areaName = (string)urlHelper.RequestContext.RouteData.DataTokens["area"];
            var areaPath = !string.IsNullOrEmpty(areaName) ? "Areas/" + areaName + "/" : string.Empty;
            
            return urlHelper.Content("~/" + areaPath + contentPath);
        }
    }
}