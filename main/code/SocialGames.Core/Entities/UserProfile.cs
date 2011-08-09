namespace Microsoft.Samples.SocialGames.Entities
{
    using System.Collections.Generic;

    public class UserProfile
    {
        public UserProfile()
        {
            this.Achievements = new List<AchievementBase>();
            this.Inventory = new List<InventoryItem>();
            this.Customizations = new Dictionary<string, InventoryItem>();
        }

        public string Id { get; set; }

        public string LoginType { get; set; }

        public string DisplayName { get; set; }

        public int Credits { get; set; }

        public int Experience { get; set; }

        public int Ranking { get; set; }

        public IDictionary<string, InventoryItem> Customizations { get; set; }

        public List<InventoryItem> Inventory { get; set; }

        public object AssociatedUserAccount { get; set; }

        public List<AchievementBase> Achievements { get; set; }

        public override bool Equals(object otherObject)
        {
            if (otherObject is UserProfile)
            {
                return this.Equals((UserProfile)otherObject);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(UserProfile user)
        {
            return this.Id == user.Id;
        }
    }
}