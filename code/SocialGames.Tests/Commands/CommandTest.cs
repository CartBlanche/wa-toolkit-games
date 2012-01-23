namespace Microsoft.Samples.SocialGames.Tests.Commands
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Samples.SocialGames.Common.JobEngine;

    using Moq;

    public abstract class CommandTest
    {
        protected Mock<IWorkerContext> CreateMockWorkerContext()
        {
            var workerContext = new Mock<IWorkerContext>();
            var contextDictionary = new ConcurrentDictionary<string, object>();
            workerContext.SetupGet(p => p.Context).Returns(contextDictionary);
            return workerContext;
        }
    }
}
