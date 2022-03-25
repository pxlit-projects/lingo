using Lingo.Domain.Pit.Contracts;
using Lingo.Domain.Puzzle.Contracts;

namespace Lingo.Domain.Contracts
{
    /// <summary>
    /// A game between 2 players.
    /// A game can contain multiple puzzles (of which one is the current puzzle).
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Unique identifier of the game
        /// </summary>
        Guid Id { get; }

        IPlayer Player1 { get; }
        IPlayer Player2 { get; }

        /// <summary>
        /// The unique identifier of the player who's turn it is
        /// </summary>
        Guid PlayerToPlayId { get; }

        /// <summary>
        /// The current puzzle that is being played in the game.
        /// The current puzzle changes when a puzzle is finished and the player (who's turn it is) cannot grab any balls from the ball pit anymore.
        /// </summary>
        IPuzzle CurrentPuzzle { get; }

        /// <summary>
        /// True if all puzzles are finished and none of the players can grab balls from the ball pit.
        /// False otherwise
        /// </summary>
        bool Finished { get; }

        /// <summary>
        /// Submits an answer of a player for the <see cref="CurrentPuzzle"/>
        /// </summary>
        /// <param name="playerId">The unique identifier of the player that submits the answer</param>
        /// <param name="answer">The answer to be submitted</param>
        /// <returns>A submission result that indicated if the player lost its turn by submitting the answer</returns>
        /// <exception cref="ApplicationException">Thrown when the player is not allowed to submit an answer</exception>
        SubmissionResult SubmitAnswer(Guid playerId, string answer);

        /// <summary>
        /// Lets a player grab a ball from its ball pit
        /// </summary>
        /// <param name="playerId">The unique identifier of the player for which a ball must be grabbed</param>
        /// <returns>
        /// The grabbed ball
        /// </returns>
        /// <exception cref="ApplicationException">Thrown when the player is not allowed to grab a ball</exception>
        IBall GrabBallFromBallPit(Guid playerId);
    }
}