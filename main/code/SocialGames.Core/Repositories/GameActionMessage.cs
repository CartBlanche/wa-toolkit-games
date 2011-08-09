namespace Microsoft.Samples.SocialGames.Repositories
{
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.Samples.SocialGames.Entities;

    public class GameActionMessage : AzureQueueMessage
    {
        public GameAction GameAction { get; set; }
    }
}