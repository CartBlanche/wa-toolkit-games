namespace Microsoft.Samples.SocialGames.Entities
{
    using System;

    public class AchievementProgress<T>
    {
        public Achievement<T> Achievement { get; set; }

        public T Progress { get; set; }
    }
}