using System;
using System.Collections.Generic;
using System.Linq;
using Guts.Client.Core;
using Lingo.AppLogic.Contracts;
using Lingo.Domain;
using Lingo.Domain.Contracts;
using Lingo.Domain.Pit.Contracts;
using Lingo.Domain.Puzzle.Contracts;
using Lingo.TestTools;
using Lingo.TestTools.Builders;
using Moq;
using NUnit.Framework;

namespace Lingo.AppLogic.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Lingo", "GameService", @"Lingo.AppLogic\GameService.cs")]
    public class GameServiceTests : TestBase
    {
        private Mock<IGameFactory> _gameFactoryMock;
        private Mock<IGameRepository> _gameRepositoryMock;
        private Mock<IPuzzleService> _puzzleServiceMock;

        private GameService _service;
        private GameSettings _gameSettings;
        private IGame _game;
        private Mock<IGame> _gameMock;

        [SetUp]
        public void Setup()
        {
            _gameMock = new GameMockBuilder().Mock;
            _game = _gameMock.Object;

            _gameFactoryMock = new Mock<IGameFactory>();
            _gameFactoryMock.Setup(factory =>
                factory.CreateStandardGameForUsers(It.IsAny<User>(), It.IsAny<User>(), It.IsAny<IList<IPuzzle>>())).Returns(_game);

            _gameRepositoryMock = new Mock<IGameRepository>();
            _gameRepositoryMock.Setup(repo => repo.GetById(It.IsAny<Guid>())).Returns(_game);

            _puzzleServiceMock = new Mock<IPuzzleService>();
            _puzzleServiceMock.Setup(service => service.CreateStandardWordPuzzle(It.IsAny<int>()))
                .Returns(new WordPuzzleMockBuilder().Object);

            _service = new GameService(
                _gameRepositoryMock.Object,
                _gameFactoryMock.Object,
                _puzzleServiceMock.Object);

            _gameSettings = new GameSettings();
        }

        [MonitoredTest("CreateGameForUsers - Should create puzzles, a game and add it to the repository")]
        public void _01_CreateGameForUsers_ShouldCreatePuzzlesAGameAndAddItToTheRepository()
        {
            //Arrange
            User user1 = new UserBuilder().Build();
            User user2 = new UserBuilder().Build();

            _gameSettings.NumberOfStandardWordPuzzles = RandomGenerator.Next(2, 11);
            _gameSettings.MinimumWordLength = 4;
            _gameSettings.MaximumWordLength = 8;
            
            //Act
            _service.CreateGameForUsers(user1, user2, _gameSettings);

            //Assert
            _puzzleServiceMock.Verify(service => service.CreateStandardWordPuzzle(It.IsAny<int>()),
                Times.Exactly(_gameSettings.NumberOfStandardWordPuzzles),
                $"When the '{nameof(GameSettings.NumberOfStandardWordPuzzles)}' of the settings is '{_gameSettings.NumberOfStandardWordPuzzles}', " +
                $"the '{nameof(IPuzzleService.CreateStandardWordPuzzle)}' method of the puzzle service should have been called that many times.");

            _puzzleServiceMock.Verify(service => service.CreateStandardWordPuzzle(It.Is<int>(wordLength => _gameSettings.MinimumWordLength <= wordLength && _gameSettings.MaximumWordLength >= wordLength)),
                Times.Exactly(_gameSettings.NumberOfStandardWordPuzzles),
                $"When the '{nameof(GameSettings.MinimumWordLength)}' of the settings is '{_gameSettings.MinimumWordLength}' " +
                $"and the '{nameof(GameSettings.MaximumWordLength)}' of the settings is '{_gameSettings.MaximumWordLength}' " +
                $"the '{nameof(IPuzzleService.CreateStandardWordPuzzle)}' method of the puzzle service should have been called each time with a word length in the [{_gameSettings.MinimumWordLength}-{_gameSettings.MaximumWordLength}] range.");

            _gameFactoryMock.Verify(factory => factory.CreateStandardGameForUsers(user1, user2, It.IsAny<IList<IPuzzle>>()), Times.Once,
                "The 'CreateStandardGameForUsers' method of the 'IGameFactory' is not called with the correct users.");

            _gameFactoryMock.Verify(factory => factory.CreateStandardGameForUsers(It.IsAny<User>(), It.IsAny<User>(),
                    It.Is<IList<IPuzzle>>(
                        puzzles => puzzles != null && puzzles.OfType<IWordPuzzle>().Count() >= _gameSettings.NumberOfStandardWordPuzzles)),
                Times.Once,
                "The 'CreateStandardGameForUsers' method of the 'IGameFactory' is not called with the correct amount of puzzles. " +
                $"When the '{nameof(GameSettings.NumberOfStandardWordPuzzles)}' of the settings is '{_gameSettings.NumberOfStandardWordPuzzles}', " +
                "then there should be an equal amount of standard word puzzles in the game.");


            _gameRepositoryMock.Verify(repo => repo.Add(_game), Times.Once,
                "The 'Add' method of the 'IGameRepository' is not called correctly.");
        }

        [MonitoredTest("GetById - Should use the repository")]
        public void _02_GetById_ShouldUseTheRepository()
        {
            //Act
            IGame retrievedGame = _service.GetById(_game.Id);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the 'IGameRepository' is not called correctly.");

            Assert.That(retrievedGame, Is.SameAs(_game),
                "The game returned should be the exact same object that is returned by the repository.");
        }

        [MonitoredTest("SubmitAnswer - Should retrieve the game and submit the answer on it")]
        public void _03_SubmitAnswer_ShouldRetrieveTheGameAndSubmitTheAnswerOnIt()
        {
            //Arrange
            string answer = RandomGenerator.NextWord();
            SubmissionResult expectedResult = SubmissionResult.CreateKeepTurnResult();
            _gameMock.Setup(game => game.SubmitAnswer(It.IsAny<Guid>(), It.IsAny<string>())).Returns(expectedResult);

            //Act
            SubmissionResult result = _service.SubmitAnswer(_game.Id, _game.PlayerToPlayId, answer);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the 'IGameRepository' is not called correctly.");

            Assert.That(result, Is.SameAs(expectedResult),
                $"The result returned should be the exact same object that is returned by the '{nameof(IGame.SubmitAnswer)}' method of the retrieved game.");

            _gameMock.Verify(game => game.SubmitAnswer(_game.PlayerToPlayId, answer), Times.Once,
                $"The '{nameof(IGame.SubmitAnswer)}' method of the retrieved game is not called correctly.");
        }

        [MonitoredTest("GrabBallFromBallPit - Should retrieve the game and grab a ball")]
        public void _04_GrabBallFromBallPit_ShouldRetrieveTheGameAndGrabABall()
        {
            //Arrange
            IBall ball = new BallMockBuilder().Object;
            _gameMock.Setup(game => game.GrabBallFromBallPit(It.IsAny<Guid>())).Returns(ball);

            //Act
            IBall grabbedBall = _service.GrabBallFromBallPit(_game.Id, _game.PlayerToPlayId);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the 'IGameRepository' is not called correctly.");

            Assert.That(grabbedBall, Is.SameAs(ball),
                $"The ball returned should be the exact same object that is returned by the '{nameof(IGame.GrabBallFromBallPit)}' method of the retrieved game.");

            _gameMock.Verify(game => game.GrabBallFromBallPit(_game.PlayerToPlayId), Times.Once,
                $"The '{nameof(IGame.GrabBallFromBallPit)}' method of the retrieved game is not called correctly.");
        }

        [MonitoredTest("GetGamesFor - Should use the repository")]
        public void _05_GetGamesFor_ShouldUseTheRepository()
        {
            //Arrange
            var expectedGames = new List<IGame>
            {
                new GameMockBuilder().Object
            };
            _gameRepositoryMock.Setup(repo => repo.GetGamesOfUser(It.IsAny<Guid>())).Returns(expectedGames);

            //Act
            IList<IGame> games = _service.GetGamesFor(_game.PlayerToPlayId);

            //Assert
            _gameRepositoryMock.Verify(repo => repo.GetGamesOfUser(_game.PlayerToPlayId), Times.Once,
                "The 'GetGamesOfUser' method of the 'IGameRepository' is not called correctly.");

            Assert.That(games, Is.SameAs(expectedGames),
                "The games returned should be the exact same list that is returned by the repository.");
        }
    }
}