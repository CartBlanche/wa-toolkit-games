namespace Microsoft.Samples.SocialGames.GamePlay.Services
{
    using System;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.Samples.SocialGames.Entities;

    [ServiceContract]
    public interface IUserService
    {
        [WebInvoke(Method = "POST", UriTemplate = "profile")]
        HttpResponseMessage UpdateProfile(HttpRequestMessage request);

        [WebGet(UriTemplate = "verify")]
        HttpResponseMessage Verify(HttpRequestMessage request);

        [WebInvoke(Method = "POST", UriTemplate = "inventory/buy/{inventoryId}")]
        HttpResponseMessage BuyInventory(Guid inventoryId, HttpRequestMessage request);

        [WebInvoke(Method = "GET", UriTemplate = "leaderboard/{count}")]
        HttpResponseMessage Leaderboard(int count);

        [WebInvoke(Method = "GET", UriTemplate = "leaderboard/{focusCount}?user={focusUserId}")]
        HttpResponseMessage LeaderboardWithFocus(string focusUserId, int focusCount);
    }
}