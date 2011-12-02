namespace Microsoft.Samples.SocialGames.Entities
{
    using System;

    public class UserSession
    {
        public string UserId { get; set; }

        public Guid ActiveGameQueueId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Equals((UserSession)obj);
        }

        public bool Equals(UserSession userSession)
        {
            return this.UserId == userSession.UserId && this.ActiveGameQueueId == userSession.ActiveGameQueueId;
        }

        public override int GetHashCode()
        {
            return this.UserId.GetHashCode() ^ this.ActiveGameQueueId.GetHashCode();
        }
    }
}