namespace Microsoft.Samples.SocialGames.Tests.Repositories
{
    using Microsoft.Samples.SocialGames.Entities;
    using Microsoft.Samples.SocialGames.Repositories;

    public static class IGameRepositoryExtensions
    {
        public static void AddUserToSkirmishGameQueue(this IGameRepository gameRepository, UserProfile userProfile)
        {
            gameRepository.AddUserToGameQueue(userProfile.Id, GameType.Skirmish);
        }
    }
}