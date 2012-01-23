namespace Microsoft.Samples.SocialGames.Worker.Commands
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.SocialGames.Common.JobEngine;
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public abstract class GameActionCommand : ICommand
    {
        public void Do(IDictionary<string, object> context)
        {
            var gameAction = (GameAction)context["gameAction"];
            this.Do(gameAction);
        }

        public abstract void Do(GameAction gameAction);
    }
}