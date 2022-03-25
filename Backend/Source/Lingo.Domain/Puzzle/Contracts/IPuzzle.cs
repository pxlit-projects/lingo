using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;

namespace Lingo.Domain.Puzzle.Contracts
{
    /// <summary>
    /// A puzzle played in a LINGO game.
    /// </summary>
    [SwaggerSubType(typeof(StandardWordPuzzle), DiscriminatorValue = nameof(Type))]
    public interface IPuzzle
    {
        /// <summary>
        /// Unique identifier of the puzzle
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Indicates if the puzzle is finished.
        /// Attention: this does not necessarily means that the puzzle is solved. Inspect the <see cref="Score"/> property to see if the puzzle is solved.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// The score of the puzzle (taking the submitted answers into account).
        /// </summary>
        int Score { get; }

        /// <summary>
        /// The type of the puzzle (for the minimal requirements this should always be 'StandardWordPuzzle')
        /// </summary>
        [DefaultValue(nameof(StandardWordPuzzle))]
        string Type { get; }

        /// <summary>
        /// Submits an answer to solve the puzzle.
        /// </summary>
        /// <param name="answer">The answer being submitted.</param>
        /// <returns>
        /// A submission result indicating if the player lost its turn because of the submission.
        /// If the player lost its turn, the submission result will contain the reason.
        /// </returns>
        SubmissionResult SubmitAnswer(string answer);

        /// <summary>
        /// Reveals a part of the puzzle.
        /// Should be called when one player makes a mistake and the turn is given to the opponent.
        /// </summary>
        void RevealPart();
    }
}