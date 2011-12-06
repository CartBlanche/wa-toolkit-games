namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System.Collections.Concurrent;

    public class InMemoryWorkerContext : IWorkerContext
    {
        private static ConcurrentDictionary<string, object> context = new ConcurrentDictionary<string, object>();

        public ConcurrentDictionary<string, object> Context
        {
            get { return context; }
        }
    }
}