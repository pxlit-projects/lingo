
using Lingo.AppLogic.Contracts;
using Lingo.Common;

namespace Lingo.Infrastructure.Storage
{
    /// <summary>
    /// Holds different sets of word dictionaries (each word dictionary contains all known words of a certain length).
    /// When a word dictionary is requested for the first time, it is loaded from a file and then stored in-memory (cache).
    /// When a word dictionary is requested for a second time, it will be retrieved from the in-memory cache.
    /// </summary>
    /// <remarks>
    /// There should be no need to alter any code in this class.
    /// The word dictionary files can be found in the Lingo.Api layer in the "WordDictionaries" folder. Those files were created using data from https://www.opentaal.org
    /// </remarks>
    internal class InMemoryWordDictionaryRepository : IWordDictionaryRepository
    {
        private Dictionary<int, HashSet<string>> _allWordDictionaries;

        public InMemoryWordDictionaryRepository()
        {
            _allWordDictionaries = new Dictionary<int, HashSet<string>>();
        }

        public HashSet<string> GetWordDictionary(int wordLength)
        {
            if (wordLength < 4 || wordLength > 10)
            {
                throw new DataNotFoundException();
            }

            if (_allWordDictionaries.ContainsKey(wordLength))
            {
                return _allWordDictionaries[wordLength];
            }

            //Load from file (created from data at https://www.opentaal.org/ )
            string[] words = File.ReadAllLines($"WordDictionaries\\{wordLength}-letter-words.txt");
            HashSet<string> wordDictionary = new HashSet<string>(words);
            _allWordDictionaries[wordLength] = wordDictionary;
            return wordDictionary;
        }
    }
}