namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System;
    using System.Collections.Generic;

    public interface ITriggeredByBuilder<TMessage>
    {
        IDoBuilder Do(params ICommand[] command);

        IDoBuilder SetupContext(Action<TMessage, IDictionary<string, object>> setupContextCallback);
    }
}