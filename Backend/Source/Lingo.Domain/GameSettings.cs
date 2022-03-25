using System.ComponentModel;

namespace Lingo.Domain
{
    public class GameSettings
    {
        /// <summary>
        /// The number of classic LINGO word puzzles in the game.
        /// </summary>
        [DefaultValue(4)]
        public int NumberOfStandardWordPuzzles { get; set; }

        /// <summary>
        /// The minimum length of the words when playing
        /// </summary>
        [DefaultValue(5)]
        public int MinimumWordLength { get; set; }

        /// <summary>
        /// The maximum length of the words when playing.
        /// </summary>
        [DefaultValue(5)]
        public int MaximumWordLength { get; set; }


        public GameSettings()
        {
            NumberOfStandardWordPuzzles = 4;
            MinimumWordLength = 5;
            MaximumWordLength = 5;
        }
    }
}