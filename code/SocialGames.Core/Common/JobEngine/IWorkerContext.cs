namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System.Collections.Concurrent;

    public interface IWorkerContext
    {
        ConcurrentDictionary<string, object> Context { get; }
    }
}