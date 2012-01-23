namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System;
    using Microsoft.Samples.SocialGames.Common.Storage;
    using Microsoft.WindowsAzure;

    public static class Message
    {
        public static CheckConditionWithAcceptCallback<TMessage> OfType<TMessage>(CloudStorageAccount account, string queueName) where TMessage : AzureQueueMessage
        {
            return OfType<TMessage>(new AzureQueue<TMessage>(account, queueName));
        }

        public static CheckConditionWithAcceptCallback<TMessage> OfType<TMessage>(IAzureQueue<TMessage> queue) where TMessage : AzureQueueMessage
        {
            var messageQueueCondition = new MessageOfTypeQueueCondition<TMessage>(queue);
            return new CheckConditionWithAcceptCallback<TMessage>
            {
                CheckFunc = messageQueueCondition.GetMessageFunc,
                ConfirmFunc = messageQueueCondition.DequeueFunc
            };
        }

        private class MessageOfTypeQueueCondition<TMessage> where TMessage : AzureQueueMessage
        {
            // TODO: Set visibiliy timeout for message (5 to 10 minutes should be OK)
            private static readonly System.TimeSpan MessageDefaultTimeout = TimeSpan.FromMinutes(2);

            private IAzureQueue<TMessage> queue;

            public MessageOfTypeQueueCondition(IAzureQueue<TMessage> queue)
            {
                this.queue = queue;
            }

            public Func<bool, TMessage> GetMessageFunc
            {
                get
                {
                    return (out TMessage output) =>
                    {
                        output = null;

                        var message = this.queue.GetMessage(MessageDefaultTimeout);
                        if (message != null)
                        {
                            output = message;
                            return true;
                        }

                        return false;
                    };
                }
            }

            public System.Action<TMessage> DequeueFunc
            {
                get
                {
                    return message => this.queue.DeleteMessage(message);
                }
            }
        }
    }
}