namespace Microsoft.Samples.SocialGames.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class GameActionType
    {
        public const int Chat = 0;
        public const int ExpireUser = 1;
        public const int Shot = 2;
        public const int Shield = 3;
        public const int EndGame = 4;
        public const int PlayerDead = 5;
        public const int ShotBot = 6;
        public const int PlayerDeadBot = 7;
        public const int EndGameBot = 8;
    }
}
