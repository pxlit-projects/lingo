using System.Collections.Generic;
using Guts.Client.Core;
using Lingo.Domain.Card.Contracts;
using Lingo.Domain.Contracts;
using Lingo.Domain.Puzzle.Contracts;
using Lingo.TestTools;
using Lingo.TestTools.Builders;
using Moq;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Lingo", "GameFactory", @"Lingo.Domain\GameFactory.cs")]
    public class GameFactoryTests : TestBase
    {
        private IGameFactory _factory;
        private Mock<ILingoCardFactory> _lingoCardFactoryMock;
        private List<IPuzzle> _puzzles;

        [SetUp]
        public void BeforeEachTest()
        {
            ILingoCard lingoCard = new LingoCardMockBuilder().Object;
            _lingoCardFactoryMock = new Mock<ILingoCardFactory>();
            _lingoCardFactoryMock.Setup(factory => factory.CreateNew(It.IsAny<bool>())).Returns(lingoCard);
            
            _puzzles = new List<IPuzzle>
            {
                new WordPuzzleMockBuilder().Object
            };
            _factory = new GameFactory(_lingoCardFactoryMock.Object) as IGameFactory;
        }

        [MonitoredTest("Should implement IGameFactory")]
        public void _01_ShouldImplementIGameFactory()
        {
            //Assert
            Assert.That(_factory, Is.Not.Null);
        }

        [MonitoredTest("CreateNewForUsers - Should create game with 2 human players")]
        public void _02_CreateStandardGameForUsers_ShouldCreateGameWith2HumanPlayers()
        {
            //Arrange
            User user1 = new UserBuilder().Build();
            User user2 = new UserBuilder().Build();

            //Act
            IGame game = _factory.CreateStandardGameForUsers(user1, user2, _puzzles);

            //Assert
            Assert.That(game.Player1, Is.InstanceOf<Player>(), "Player 1 should be an instance of 'Player'.");
            Assert.That(game.Player1.Id, Is.EqualTo(user1.Id), "The 'Id' of player 1 should be the id of user 1.");
            Assert.That(game.Player1.Name, Is.EqualTo(user1.NickName), "The 'Name' of player 1 should be the nickname of user 1.");
            Assert.That(game.Player1.BallPit, Is.Not.Null, "The 'BallPit' of player 1 should not be null.");

            Assert.That(game.Player2, Is.InstanceOf<Player>(), "Player 2 should be an instance of 'Player'.");
            Assert.That(game.Player2.Id, Is.EqualTo(user2.Id), "The 'Id' of player 2 should be the id of user 2.");
            Assert.That(game.Player2.Name, Is.EqualTo(user2.NickName), "The 'Name' of player 2 should be the nickname of user 2.");
            Assert.That(game.Player2.BallPit, Is.Not.Null, "The 'BallPit' of player 2 should not be null.");

            _lingoCardFactoryMock.Verify(factory => factory.CreateNew(true), Times.Once,
                "One of the players must use even numbers. " +
                "When the player was created, " +
                $"the '{nameof(Player)}' constructor should have called the '{nameof(ILingoCardFactory.CreateNew)}' method of the card factory with 'useEvenNumbers' true.");

            _lingoCardFactoryMock.Verify(factory => factory.CreateNew(false), Times.Once,
                "One of the players must use odd numbers. " +
                "When the player was created, " +
                $"the '{nameof(Player)}' constructor should have called the '{nameof(ILingoCardFactory.CreateNew)}' method of the card factory with 'useEvenNumbers' false.");
        }
    }
}
