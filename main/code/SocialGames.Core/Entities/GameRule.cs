namespace Microsoft.Samples.SocialGames.Entities
{
    using System;
    using System.Collections.Generic;

    public class GameRule : Tuple<string, object>
    {
        public GameRule(string item1, object item2) 
            : base(item1, item2)
        { 
        }
    }
}