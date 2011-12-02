namespace Microsoft.Samples.SocialGames.Helpers
{
    using Microsoft.Samples.SocialGames.Common;
    using Microsoft.Samples.SocialGames.Entities;

    public static class GameExtensions
    {
        public static double TimeElapsed(this GameQueue gameQueue)
        {
            return (TimeProvider.Current.CurrentDateTime - gameQueue.CreationTime).TotalSeconds;
        }
    }
}