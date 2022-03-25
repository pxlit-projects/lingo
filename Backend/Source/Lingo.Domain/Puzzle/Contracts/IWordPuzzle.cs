namespace Lingo.Domain.Puzzle.Contracts
{
    /// <inheritdoc cref="IPuzzle"  />
    public interface IWordPuzzle : IPuzzle
    {
        /// <summary>
        /// The length of the solution word.
        /// </summary>
        int WordLength { get; }

        /// <summary>
        /// The guesses that where already made in the puzzle.
        /// </summary>
        IReadOnlyList<WordGuess> Guesses { get; }

        /// <summary>
        /// The letters that are revealed by having a correct match in one of the guesses.
        /// If a letter is not revealed yet, is has a dot (.) as value.
        /// </summary>
        char[] RevealedLetters { get; }
    }
}