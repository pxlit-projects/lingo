using Lingo.Domain.Puzzle.Contracts;

namespace Lingo.Domain.Puzzle
{
    /// <inheritdoc cref="IPuzzleFactory"/>
    internal class PuzzleFactory : IPuzzleFactory
    {
        public IWordPuzzle CreateStandardWordPuzzle(HashSet<string> wordDictionary)
        {
            throw new NotImplementedException();
        }
    }
}