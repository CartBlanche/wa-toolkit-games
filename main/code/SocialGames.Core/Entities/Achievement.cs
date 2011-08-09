namespace Microsoft.Samples.SocialGames.Entities
{
    using System;

    public class Achievement<T> : AchievementBase
    {
        public uint Id { get; set; }

        public Weapon[] RewardedWeapons { get; set; }

        public int RewardedResources { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string ConditionedField { get; set; }

        public int LevelToAchieve { get; set; }
    }
}