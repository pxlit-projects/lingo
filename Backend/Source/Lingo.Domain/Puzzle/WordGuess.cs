namespace Lingo.Domain.Puzzle
{
    /// <summary>
    /// A guess made in a word puzzle.
    /// Contains the guessed word and the letter matches compared to the solution
    /// </summary>
    public class WordGuess
    {
        /// <summary>
        /// The guessed word.
        /// </summary>
        public string Word { get; }

        /// <summary>
        /// Matches of the letters in the guessed <see cref="Word"/> compared to the solution.
        /// Each letter gets one of the following values:
        /// 1 (Correct): the letter matches exactly with the letter in the solution on the same position.
        /// 0 (CorrectButInWrongPosition): the letter occurs in the solution but on a different position (and is not yet matched with a letter in the solution on the correct position).
        /// -1 (DoesNotOccur): the letter does not occur on any position in the solution. 
        /// </summary>
        public LetterMatch[] LetterMatches { get; }

        public WordGuess(string word, string solution)
        {
            
        }
    }
}