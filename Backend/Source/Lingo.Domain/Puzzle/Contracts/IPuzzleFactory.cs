namespace Lingo.Domain.Puzzle.Contracts
{
    /// <summary>
    /// Factory that can create instances of classes that implement <see cref="IPuzzle"/>
    /// </summary>
    public interface IPuzzleFactory
    {
        /// <summary>
        /// Creates a <see cref="StandardWordPuzzle"/> by selecting a random word from a <paramref name="wordDictionary"/> as the solution
        /// </summary>
        /// <param name="wordDictionary">A set of all possible words that could be a solution in the puzzle</param>
        /// <returns>The created puzzle</returns>
        IWordPuzzle CreateStandardWordPuzzle(HashSet<string> wordDictionary);
    }
}