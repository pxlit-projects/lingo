using System.Collections.Generic;
using Guts.Client.Core;
using Lingo.AppLogic.Contracts;
using Lingo.Domain.Puzzle.Contracts;
using Lingo.TestTools;
using Lingo.TestTools.Builders;
using Moq;
using NUnit.Framework;

namespace Lingo.AppLogic.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Lingo", "PuzzleService", @"Lingo.AppLogic\PuzzleService.cs")]
    public class PuzzleServiceTests : TestBase
    {
        private Mock<IPuzzleFactory> _puzzleFactoryMock;
        private Mock<IWordDictionaryRepository> _wordDictionaryRepositoryMock;
        
        private PuzzleService _service;
        
        private IWordPuzzle _wordPuzzle;
        private Mock<IWordPuzzle> _wordPuzzleMock;

        private HashSet<string> _wordDictionary;

        [SetUp]
        public void Setup()
        {
            _wordPuzzleMock = new WordPuzzleMockBuilder().Mock;
            _wordPuzzle = _wordPuzzleMock.Object;

            _puzzleFactoryMock = new Mock<IPuzzleFactory>();
            _puzzleFactoryMock.Setup(factory =>
                factory.CreateStandardWordPuzzle(It.IsAny<HashSet<string>>())).Returns(_wordPuzzle);

            _wordDictionary = new HashSet<string>();
            _wordDictionaryRepositoryMock = new Mock<IWordDictionaryRepository>();
            _wordDictionaryRepositoryMock.Setup(repo => repo.GetWordDictionary(It.IsAny<int>())).Returns(_wordDictionary);

            _service = new PuzzleService(_wordDictionaryRepositoryMock.Object, _puzzleFactoryMock.Object);
        }

        [MonitoredTest("CreateStandardWordPuzzle - Should retrieve a word dictionary and use it to create the puzzle")]
        public void _01_CreateStandardWordPuzzle_ShouldRetrieveAWordDictionaryAndUseItToCreateThePuzzle()
        {
            //Arrange
            int wordLength = RandomGenerator.Next(4, 11);
            
            //Act
            IWordPuzzle createdPuzzle = _service.CreateStandardWordPuzzle(wordLength);

            //Assert
            _wordDictionaryRepositoryMock.Verify(repo => repo.GetWordDictionary(wordLength), Times.Once,
                "The 'GetWordDictionary' method of the 'IWordDictionaryRepository' is not called correctly.");

            _puzzleFactoryMock.Verify(factory => factory.CreateStandardWordPuzzle(_wordDictionary), Times.Once,
                "The 'CreateStandardWordPuzzle' method of the 'IPuzzleFactory' is not called correctly.");

            Assert.That(createdPuzzle, Is.SameAs(_wordPuzzle),
                "The returned puzzle should be the exact same object that is returned by the factory.");
        }
    }
}