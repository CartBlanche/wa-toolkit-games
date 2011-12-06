namespace Microsoft.Samples.SocialGames.Entities
{

    public class UserProfile
    {
        public UserProfile()
        {
        }

        public string Id { get; set; }

        public string LoginType { get; set; }

        public string DisplayName { get; set; }

        public object AssociatedUserAccount { get; set; }

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