namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System.Collections.Generic;

    public interface ICommand
    {
        void Do(IDictionary<string, object> context);
    }
}