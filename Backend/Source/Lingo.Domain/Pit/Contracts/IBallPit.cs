using Lingo.Domain.Card.Contracts;

namespace Lingo.Domain.Pit.Contracts
{
    /// <summary>
    /// Pit that contains balls that can be grabbed
    /// </summary>
    public interface IBallPit
    {
        /// <summary>
        /// Fills the pit with 3 red balls and a blue numbered ball for each number on the <paramref name="lingoCard"/> that is not crossed out.
        /// </summary>
        /// <param name="lingoCard">The card that determines which blue balls are added to the pit</param>
        void FillForLingoCard(ILingoCard lingoCard);

        /// <summary>
        /// Randomly grabs a ball from the pit.
        /// Red balls stay in the pit (they are put back).
        /// Blue balls are removed from the pit. 
        /// </summary>
        /// <returns>The grabbed ball</returns>
        IBall GrabBall();
    }
}