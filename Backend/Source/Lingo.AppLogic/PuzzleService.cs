using Lingo.AppLogic.Contracts;
using Lingo.Domain.Puzzle.Contracts;

namespace Lingo.AppLogic;

/// <inheritdoc cref="IPuzzleService"/>
internal class PuzzleService : IPuzzleService
{
    public PuzzleService(IWordDictionaryRepository wordDictionaryRepository, IPuzzleFactory puzzleFactory)
    {

    }

    public IWordPuzzle CreateStandardWordPuzzle(int wordLength)
    {
        throw new NotImplementedException();
    }
}