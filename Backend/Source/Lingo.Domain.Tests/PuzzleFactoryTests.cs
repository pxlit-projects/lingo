using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guts.Client.Core;
using Lingo.Domain.Puzzle;
using Lingo.Domain.Puzzle.Contracts;
using Lingo.TestTools;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    public class PuzzleFactoryTests : TestBase
    {

        private HashSet<string> _wordDictionary;
        private IPuzzleFactory _factory;
        private readonly FieldInfo _solutionField;
        private readonly FieldInfo _dictionaryField;

        public PuzzleFactoryTests()
        {
            _solutionField = typeof(StandardWordPuzzle).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(f => f.FieldType == typeof(string) && f.Name.ToLower().Contains("solution"));

            _dictionaryField = typeof(StandardWordPuzzle).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(f => f.FieldType == typeof(HashSet<string>));
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _wordDictionary = new HashSet<string>();
            _factory = new PuzzleFactory() as IPuzzleFactory;
        }

        [MonitoredTest("Should implement PuzzleFactory")]
        public void _01_ShouldImplementIPuzzleFactory()
        {
            //Assert
            Assert.That(_factory, Is.Not.Null);
        }

        [MonitoredTest("CreateStandardWordPuzzle - Should randomly pick a word from the dictionary and create a standard word puzzle")]
        public void _02_CreateStandardWordPuzzle_ShouldRandomlyPickAWordFromTheDictionaryAndCreateAStandardWordPuzzle()
        {
            Assert.That(_solutionField, Is.Not.Null,
                $"Please make sure that the '{nameof(StandardWordPuzzle)}' uses a private backing field to store the solution word. " +
                "The field should be a string and have the name 'solution' or '_solution'. " +
                $"This test will read that backing field to check which word was passed to the constructor of '{nameof(StandardWordPuzzle)}'.");

            Assert.That(_dictionaryField, Is.Not.Null,
                $"Please make sure that the '{nameof(StandardWordPuzzle)}' uses a private backing field to store the word dictionary. " +
                "The field should be a HashSet<string>. " +
                $"This test will read that backing field to check if the word dictionary passed to the constructor of '{nameof(StandardWordPuzzle)}' is used.");

            int wordLength = RandomGenerator.Next(4, 11);
            int numberOfWords = 100;
            for (int i = 0; i < numberOfWords; i++)
            {
                _wordDictionary.Add(RandomGenerator.NextWord(wordLength));
            }

            string previousSolution = string.Empty;
            int numberOfPuzzles = 10;
            int numberOfTimesTheSameWordIsPicked = 0;
            for (int i = 0; i < numberOfPuzzles; i++)
            {
                StandardWordPuzzle puzzle = _factory.CreateStandardWordPuzzle(_wordDictionary) as StandardWordPuzzle;

                Assert.That(puzzle, Is.Not.Null, $"The created puzzle cannot be null and should be of type '{nameof(StandardWordPuzzle)}'.");
                string solution = _solutionField.GetValue(puzzle) as string;
                Assert.That(_wordDictionary.Contains(solution), Is.True,
                    $"The solution word passed into the constructor of '{nameof(StandardWordPuzzle)}' must be a word from the dictionary.");
                HashSet<string> wordDictionary = _dictionaryField.GetValue(puzzle) as HashSet<string>;
                Assert.That(wordDictionary, Is.SameAs(_wordDictionary),
                    $"The word dictionary passed into the constructor of '{nameof(StandardWordPuzzle)}' must be the same object stored in the private dictionary field of the puzzle.");

                if (solution == previousSolution)
                {
                    numberOfTimesTheSameWordIsPicked++;
                }

                previousSolution = solution;
            }

            Assert.That(numberOfTimesTheSameWordIsPicked, Is.LessThan(numberOfPuzzles * 0.2),
                "The picking of a word from the dictionary does not seem random enough. " +
                $"Often the same word is picked in a dictionary of {numberOfWords} words.");
        }
    }
}
