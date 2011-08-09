namespace Microsoft.Samples.SocialGames.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GameActionProcessor
    {
        private IList<ConditionAction> actions = new List<ConditionAction>();

        public void Register(Predicate<GameAction> condition, Action<GameAction> action)
        {
            this.actions.Add(new ConditionAction() { Condition = condition, Action = action });
        }

        public void Register(int type, Action<GameAction> action)
        {
            this.Register(gc => gc.Type == type, action);
        }

        public void Process(GameAction gameAction)
        {
            foreach (var action in this.actions)
            {
                if (action.Condition(gameAction))
                {
                    action.Action(gameAction);
                }
            }
        }

        private class ConditionAction
        {
            internal Predicate<GameAction> Condition { get; set; }

            internal Action<GameAction> Action { get; set; }
        }
    }
}
