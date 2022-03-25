using Lingo.Domain.Puzzle.Contracts;

namespace Lingo.Domain.Puzzle
{
    /// <summary>
    /// Puzzle in which letters of a word to be solved, are revealed.
    /// When all letters are revealed the puzzle is solved.
    /// Multiple guesses can be made to solve the puzzle.
    /// </summary>
    /// <inheritdoc cref="IWordPuzzle"/>
    internal class StandardWordPuzzle
    {
        /// <summary>
        /// Constructs a word puzzle
        /// </summary>
        /// <param name="solution">The solution of the puzzle</param>
        /// <param name="wordDictionary">The dictionary of words that should be used to verify submitted answers</param>
        public StandardWordPuzzle(string solution, HashSet<string> wordDictionary)
        {

        }
    }
}