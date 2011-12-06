namespace Microsoft.Samples.SocialGames.Tests
{
    using System.Net;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class MyAsserts
    {
        public static void IsBadRequest(HttpResponseMessage message, string description)
        {
            Assert.AreEqual(HttpStatusCode.BadRequest, message.StatusCode);
            Assert.AreEqual(description, message.Content.ReadAsStringAsync().Result);
        }
    }
}