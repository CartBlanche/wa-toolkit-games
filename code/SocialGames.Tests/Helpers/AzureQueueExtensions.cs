namespace Microsoft.Samples.SocialGames.Tests
{
    using Microsoft.Samples.SocialGames.Common.Storage;

    public static class AzureQueueExtensions
    {
        public static T PopMessage<T>(this AzureQueue<T> azureQueue) where T : AzureQueueMessage
        {
            var message = azureQueue.GetMessage();
            azureQueue.DeleteMessage(message);
            return message;
        }
    }
}