namespace Microsoft.Samples.SocialGames.Common.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class AzureQueue<T> : IAzureQueue<T> where T : AzureQueueMessage
    {
        private readonly CloudQueue queue;

        public AzureQueue(CloudStorageAccount account)
            : this(account, typeof(T).Name.ToLowerInvariant())
        {
        }

        public AzureQueue(CloudStorageAccount account, string queueName)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            var client = account.CreateCloudQueueClient();
            this.queue = client.GetQueueReference(queueName);
        }

        public void AddMessage(T message)
        {
            string serializedMessage = new JavaScriptSerializer().Serialize(message);
            this.queue.AddMessage(new CloudQueueMessage(serializedMessage));
        }

        public T GetMessage()
        {
            return this.GetMessageInternal(null);
        }

        public T GetMessage(TimeSpan timeout)
        {
            return this.GetMessageInternal(timeout);
        }

        public IEnumerable<T> GetMessages(int maxMessagesToReturn)
        {
            var messages = this.queue.GetMessages(maxMessagesToReturn);

            foreach (var message in messages)
            {
                yield return GetDeserializedMessage(message);
            }
        }

        public void EnsureExist()
        {
            this.queue.CreateIfNotExist();
        }

        public void Clear()
        {
            this.queue.Clear();
        }

        public void DeleteMessage(T message)
        {
            this.queue.DeleteMessage(message.Id, message.PopReceipt);
        }

        public void DeleteQueue()
        {
            this.queue.Delete();
        }

        private static T GetDeserializedMessage(CloudQueueMessage message)
        {
            var deserializedMessage = new JavaScriptSerializer().Deserialize<T>(message.AsString);
            deserializedMessage.Id = message.Id;
            deserializedMessage.PopReceipt = message.PopReceipt;
            deserializedMessage.DequeueCount = message.DequeueCount;

            return deserializedMessage;
        }

        private T GetMessageInternal(TimeSpan? timeout)
        {
            CloudQueueMessage message;
            if (timeout.HasValue)
            {
                message = this.queue.GetMessage(timeout.Value);
            }
            else
            {
                message = this.queue.GetMessage();
            }

            if (message == null)
            {
                return default(T);
            }

            return GetDeserializedMessage(message);
        }
    }
}