namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames.Common.Storage;

    public class LeaveGameMessage : AzureQueueMessage
    {
        public string UserId { get; set; }

        public Guid GameId { get; set; }
    }
}