namespace Microsoft.Samples.SocialGames.Common
{
    using System;

    public interface ITimeProvider
    {
        DateTime CurrentDateTime { get; }
    }
}