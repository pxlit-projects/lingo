using Lingo.Domain.Puzzle.Contracts;

namespace Lingo.Domain.Contracts
{
    /// <summary>
    /// Factory that can create instances of classes that implement <see cref="IGame"/>
    /// </summary>
    public interface IGameFactory
    {
        /// <summary>
        /// Creates a standard game for 2 users.
        /// </summary>
        /// <param name="user1">The user that will be player 1</param>
        /// <param name="user2">The user that will be player 2</param>
        /// <param name="puzzles">The puzzles that have to be solved during the game</param>
        /// <returns>The created game</returns>
        IGame CreateStandardGameForUsers(User user1, User user2, IList<IPuzzle> puzzles);
    }
}