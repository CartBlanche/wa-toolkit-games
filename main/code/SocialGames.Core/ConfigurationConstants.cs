namespace Microsoft.Samples.SocialGames
{
    using System;
    using System.Globalization;

    public class ConfigurationConstants
    {
        public const string GuestUserid = "guest";
        public const string UsersContainerName = "sgusers";
        public const string FriendsContainerName = "sgfriends";
        public const string UserSessionsContainerName = "sgsessions";
        public const string GamesContainerName = "sggames";
        public const string GameActionNotificationsQueue = "sgactionnotifications";
        public const string GameActionStatisticsQueue = "sgactionstatitics";
        public const string GamesQueuesContainerName = "sggamesqueues";
        public const string InventoryContainerName = "sginventory";
        public const string InventoryItemsBlobName = "sgitems";
        public const string NotificationsContainerName = "sgnotifications";
        public const string BotUserIdPrefix = "Bot-";
        public const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
        public const string FacebookAccessTokenClaimType = "http://www.facebook.com/claims/AccessToken";

        public const int MaxNumberOfPlayersPerGame = 5;

        public const int NotificationTimeInterval = 60; // in seconds

        public static TimeSpan GameQueueTimeoutWaiting
        {
            get { return TimeSpan.FromSeconds(60); }
        }

        public static TimeSpan WaitingForPlayersTimeout
        {
            get { return TimeSpan.FromSeconds(45); }
        }

        public static string GetIdentityProvidersInfoEndpoint(string acsNamespace, string realm, string returnUrl)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "https://{0}:443/v2/metadata/IdentityProviders.js?protocol=wsfederation&realm={1}&version=1.0&context={2}",
                acsNamespace,
                Uri.EscapeDataString(realm),
                Uri.EscapeDataString(returnUrl));
        }
    }
}