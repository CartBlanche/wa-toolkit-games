namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    using System;

    public static class Task
    {
        public static ITriggeredByBuilder<object> TriggeredBy(Func<bool> condition)
        {
            return new TaskBuilder<object>(condition);
        }

        public static ITriggeredByBuilder<T> TriggeredBy<T>(CheckConditionWithAcceptCallback<T> condition)
        {
            return new TaskBuilder<T>(condition);
        }
    }    
}