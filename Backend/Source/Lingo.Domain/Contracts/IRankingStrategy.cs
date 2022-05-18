namespace Lingo.Domain.Contracts;

/// <summary>
/// Contains the algorithm to rank users.
/// </summary>
public interface IRankingStrategy
{
    /// <summary>
    /// Adjusts the rank (and score) of the users ranked near the winner and loser of a game.
    /// </summary>
    /// <param name="userSlice">
    /// List of users that are ranked near the winner and loser. The ranking of these users may be impacted.
    /// All users ranked between the winner and loser are included,
    /// but also the user ranked one higher than the highest ranked user and the user ranked one lower than the lowest ranked user.
    /// The list of users is sorted by rank (descending)
    /// </param>
    /// <param name="player1User">User in the <paramref name="userSlice"/> that was player 1 in the game</param>
    /// <param name="player1GameScore">Score of <paramref name="player1User"/> in the game</param>
    /// <param name="player2User">User in the <paramref name="userSlice"/> that was player 2 in the game</param>
    /// <param name="player2GameScore">Score of <paramref name="player2User"/> in the game</param>
    void AdjustRanking(IList<User> userSlice, User player1User, int player1GameScore, User player2User, int player2GameScore);
}