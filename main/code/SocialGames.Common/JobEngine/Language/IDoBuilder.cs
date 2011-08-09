namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System;
    using System.Collections.Generic;

    public interface IDoBuilder
    {
        void Start();

        void Start(ExecutionModel execution);

        IDoBuilder Do(params ICommand[] command);

        IDoBuilder OnError(Action<ICommand, IDictionary<string, object>, Exception> call);

        IDoBuilder ContinueOnError();
    }
}