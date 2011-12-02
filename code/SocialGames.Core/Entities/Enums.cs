namespace Microsoft.Samples.SocialGames.Entities
{
    using System;

    public enum GameStatus
    {
        Waiting,        // Still waiting for all participants 
        Ready,          // Your game is ready
        Timeout,        // Not enough available people to play
        GameStarted,    // The game you want has started without you
        GameOver,       // Your game is already over
        NotFound        // Incorrect ID or game is not valid
    }

    public enum QueueStatus
    {
        Waiting,        // Still waiting for all participants
        NotFound,       // Queue id is invalid
        Ready,          // Your game is ready, user will be put in the war room for weapon selection
        Timeout         // Not enough available people to play
    }

    public enum GameType
    {
        Skirmish,
        Ranking,
        Invitational
    }
}