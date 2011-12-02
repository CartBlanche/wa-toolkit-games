namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class InMemoryWorkerContext : IWorkerContext
    {
        private static ConcurrentDictionary<string, object> context = new ConcurrentDictionary<string, object>();

        public ConcurrentDictionary<string, object> Context
        {
            get { return context; }
        }
    }
}