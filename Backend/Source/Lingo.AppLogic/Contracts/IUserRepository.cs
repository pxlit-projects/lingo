using Lingo.Common;
using Lingo.Domain;
using Lingo.Domain.Contracts;

namespace Lingo.AppLogic.Contracts
{
    /// <summary>
    /// Retrieves users from storage (e.g. a database)
    /// </summary>
    /// <remarks>
    /// Implemented by the UserDbRepository class in the Lingo.Infrastructure layer
    /// </remarks>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves the users that match the <paramref name="filter"/>.
        /// A user is returned when its username, email or nickname contains the filter (case insensitive).
        /// </summary>
        /// <param name="filter">The filter to be used. If null or empty, all users will be returned</param>
        IList<User> FindUsers(string filter);

        /// <summary>
        /// Retrieves a user from storage
        /// </summary>
        /// <param name="id">The unique identifier of the user</param>
        /// <returns>The user if it was found</returns>
        /// <exception cref="DataNotFoundException">Thrown when no matching user can be found</exception>
        User GetById(Guid id);

        /// <summary>
        /// Adjusts the ranking of the winner and loser of a game (and possibly the users ranked between them)
        /// </summary>
        /// <param name="game">The game that was played</param>
        void AdjustRankingAfterGame(IGame game);
    }
}
