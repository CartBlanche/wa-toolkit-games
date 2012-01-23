namespace Microsoft.Samples.SocialGames.Entities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Helpers;

    public class GameUser
    {
        public GameUser()
        {
            this.Weapons = new List<Guid>();
        }
        
        public string UserId { get; set; }

        public string UserName { get; set; }
        
        public List<Guid> Weapons { get; set; }

        public override bool Equals(object obj)
        {
            return (obj is GameUser) && this.Equals((GameUser)obj);
        }

        public bool Equals(GameUser gameUser)
        {
            bool result = this.UserId == gameUser.UserId &&
                   this.UserName == gameUser.UserName &&
                   new CollectionHaveSameElementsComparison<Guid>(this.Weapons, gameUser.Weapons).DoIt();

            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}