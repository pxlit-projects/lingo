namespace Lingo.AppLogic.Contracts
{
    /// <summary>
    /// Retrieves word dictionaries (a set of known words) from a storage medium (e.g. a file)
    /// </summary>
    /// <remarks>
    /// Implemented by the InMemoryWordDictionaryRepository class in the Lingo.Infrastructure layer
    /// </remarks>
    public interface IWordDictionaryRepository
    {
        /// <summary>
        /// Retrieves a set of all the known words of a certain length
        /// </summary>
        /// <param name="wordLength">The length that all the words in the set must have</param>
        HashSet<string> GetWordDictionary(int wordLength);
    }
}
