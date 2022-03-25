using Lingo.Common;
using Lingo.Domain.Contracts;

namespace Lingo.AppLogic.Contracts
{
    /// <summary>
    /// Stores and retrieves game instances in a storage medium (e.g. in server RAM memory)
    /// </summary>
    /// <remarks>
    /// Implemented by the InMemoryGameRepository class in the Lingo.Infrastructure layer
    /// </remarks>
    public interface IGameRepository
    {
        /// <summary>
        /// Adds a game
        /// </summary>
        /// <param name="newGame">The game to be stored</param>
        void Add(IGame newGame);

        /// <summary>
        /// Retrieves a game from storage
        /// </summary>
        /// <param name="id">The unique identifier of the game</param>
        /// <returns>The matching game if it exists</returns>
        /// <exception cref="DataNotFoundException">Thrown when no matching game can be found</exception>
        IGame GetById(Guid id);

        /// <summary>
        /// Retrieves the games that were created by the quiz master with the user as one of the players.
        /// </summary>
        /// <param name="userId">The unique identifier of the user that must be one of the players of the game.</param>
        IList<IGame> GetGamesOfUser(Guid userId);
    }
}