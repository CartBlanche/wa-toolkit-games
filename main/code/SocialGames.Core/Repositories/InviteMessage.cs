namespace Microsoft.Samples.SocialGames.Repositories
{
    using System;
    using Microsoft.Samples.SocialGames.Common.Storage;

    public class InviteMessage : AzureQueueMessage
    {
        public string UserId { get; set; }

        public Guid GameQueueId { get; set; }

        public string InvitedUserId { get; set; }

        public string Message { get; set; }

        public string Url { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
