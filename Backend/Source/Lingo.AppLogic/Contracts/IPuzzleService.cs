using Lingo.Domain.Puzzle.Contracts;

namespace Lingo.AppLogic.Contracts;

/// <summary>
/// Service to create puzzles
/// </summary>
public interface IPuzzleService
{
    /// <summary>
    /// Creates a standard LINGO word puzzle 
    /// </summary>
    /// <param name="wordLength">The length of the words in the puzzle (this determines which word dictionary should be used)</param>
    /// <returns>The created puzzle</returns>
    IWordPuzzle CreateStandardWordPuzzle(int wordLength);
}