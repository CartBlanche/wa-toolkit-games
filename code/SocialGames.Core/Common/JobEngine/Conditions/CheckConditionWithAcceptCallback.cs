namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System;

    public class CheckConditionWithAcceptCallback<TMessage>
    {
        public Func<bool, TMessage> CheckFunc { get; set; }

        public Action<TMessage> ConfirmFunc { get; set; }
    }
}