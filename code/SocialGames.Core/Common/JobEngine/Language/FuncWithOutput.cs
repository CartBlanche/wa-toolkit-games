namespace Microsoft.Samples.SocialGames.Common.JobEngine
{
    public delegate TResult Func<TResult, TOutput>(out TOutput output);
}