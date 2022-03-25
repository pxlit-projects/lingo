using Lingo.Common;
using Lingo.Domain;
using Lingo.Domain.Contracts;
using Lingo.Domain.Pit.Contracts;

namespace Lingo.AppLogic.Contracts
{
    /// <summary>
    /// Service to manipulate all the games in the application
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Creates and stores a new game for 2 players (users)
        /// </summary>
        /// <param name="user1">The user that will be player 1</param>
        /// <param name="user2">The user that will be player 2</param>
        /// <param name="settings">The settings that should be used when creating the game</param>
        void CreateGameForUsers(User user1, User user2, GameSettings settings);

        /// <summary>
        /// Retrieves the games that were created by the quiz master with a certain user as one of the players
        /// </summary>
        /// <param name="userId">The unique identifier of the user that must be one of the players of a game</param>
        IList<IGame> GetGamesFor(Guid userId);

        /// <summary>
        /// Retrieves a game from storage
        /// </summary>
        /// <param name="gameId">The unique identifier of the game</param>
        /// <returns>The matching game if it exists</returns>
        /// <exception cref="DataNotFoundException">Thrown when no matching game can be found</exception>
        IGame GetById(Guid gameId);

        /// <summary>
        /// Retrieves a game from storage and then submits an answer of a player to it
        /// </summary>
        /// <param name="gameId">The unique identifier of the game</param>
        /// <param name="playerId">The unique identifier of the player that submits the answer</param>
        /// <param name="answer">The answer to be submitted</param>
        /// <returns>A submission result that indicated if the player lost its turn by submitting the answer</returns>
        /// <exception cref="DataNotFoundException">Thrown when no matching game can be found</exception>
        /// <exception cref="ApplicationException">Thrown when the player is not allowed to submit an answer</exception>
        SubmissionResult SubmitAnswer(Guid gameId, Guid playerId, string answer);

        /// <summary>
        /// Retrieves a game from storage and that grab a ball from the ball pit of a player
        /// </summary>
        /// <param name="gameId">The unique identifier of the game</param>
        /// <param name="playerId">The unique identifier of the player for which a ball must be grabbed</param>
        /// <returns>
        /// The grabbed ball
        /// </returns>
        /// <exception cref="ApplicationException">Thrown when the player is not allowed to grab a ball</exception>
        /// <exception cref="DataNotFoundException">Thrown when no matching game can be found</exception>
        IBall GrabBallFromBallPit(Guid gameId, Guid playerId);
    }
}