
using Lingo.Domain.Card.Contracts;
using Lingo.Domain.Pit.Contracts;
using Lingo.Domain.Puzzle.Contracts;

namespace Lingo.Domain.Contracts
{
    /// <summary>
    /// A player in a <see cref="IGame"/>
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Unique identifier of the player
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// (Display) name of the player
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The current LINGO card of the player
        /// </summary>
        ILingoCard Card { get; }

        /// <summary>
        /// The ball pit from which the player can grab balls
        /// </summary>
        IBallPit BallPit { get; }

        /// <summary>
        /// The total score earned by the player (by solving puzzles or making a LINGO on its <see cref="Card"/>)
        /// </summary>
        int Score { get; }

        /// <summary>
        /// Indicates if the player is allowed to grab a ball from its <see cref="BallPit"/>
        /// </summary>
        bool CanGrabBallFromBallPit { get; }

        /// <summary>
        /// Adds the score of the <see cref="puzzle"/> to the <see cref="Score"/> of the player (if the puzzle is finished).
        /// Also allows the player to grab 2 balls from its <see cref="BallPit"/>.
        /// </summary>
        /// <param name="puzzle">The puzzle that was solved</param>
        void RewardForSolvingPuzzle(IPuzzle puzzle);

        /// <summary>
        /// <para>
        /// Lets the player grab a ball from its <see cref="BallPit"/> (Only if allowed due to a previous call to <see cref="RewardForSolvingPuzzle"/>).
        /// </para>
        /// <para>
        /// If a red ball is grabbed the player should not be able to grab a second ball.
        /// </para>
        /// <para>
        /// If a blue ball is grabbed that causes a LINGO on the <see cref="Card"/> of the player:
        /// <list type="bullet">
        ///     <item>
        ///         <description>the <see cref="Score"/> of the player is incremented by 100</description>
        ///     </item>
        ///     <item>
        ///         <description>the player gets a new <see cref="Card"/></description>
        ///     </item>
        ///     <item>
        ///         <description>the <see cref="BallPit"/> of the player is refilled using the numbers on the new card</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>The grabbed ball</returns>
        IBall GrabBallFromBallPit();
    }
}
