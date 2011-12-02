namespace Microsoft.Samples.SocialGames.Repositories
{
    using Microsoft.Samples.SocialGames.Common.Storage;

    public class SkirmishGameQueueMessage : AzureQueueMessage
    {
        public string UserId { get; set; }
    }
}