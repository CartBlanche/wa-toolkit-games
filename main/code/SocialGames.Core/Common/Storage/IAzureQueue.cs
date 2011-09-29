namespace Microsoft.Samples.SocialGames.Common.Storage
{
    using System;
    using System.Collections.Generic;

    public interface IAzureQueue<T> where T : AzureQueueMessage
    {
        void EnsureExist();

        void Clear();

        void AddMessage(T message);

        T GetMessage();

        T GetMessage(TimeSpan timeout);

        IEnumerable<T> GetMessages(int maxMessagesToReturn);

        void DeleteMessage(T message);

        void DeleteQueue();
    }
}