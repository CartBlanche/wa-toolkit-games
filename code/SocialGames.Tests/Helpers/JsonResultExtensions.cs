namespace Microsoft.Samples.SocialGames.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    public static class JsonResultExtensions
    {
        public static bool IsEquivalentTo(this JsonResult leftJsonResult, JsonResult rightJsonResult)
        {
            return ObjectsFieldComparison.Comparing(leftJsonResult.Data, rightJsonResult.Data).DoIt()
                   && leftJsonResult.ContentType == rightJsonResult.ContentType
                   && leftJsonResult.ContentEncoding == rightJsonResult.ContentEncoding
                   && leftJsonResult.JsonRequestBehavior == rightJsonResult.JsonRequestBehavior;
        }
    }
}
