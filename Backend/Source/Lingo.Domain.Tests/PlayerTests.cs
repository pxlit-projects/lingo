using System;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Lingo.Domain.Card.Contracts;
using Lingo.Domain.Contracts;
using Lingo.Domain.Pit;
using Lingo.Domain.Pit.Contracts;
using Lingo.Domain.Puzzle.Contracts;
using Lingo.TestTools;
using Moq;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Lingo", "Player", @"Lingo.Domain\Player.cs")]
    public class PlayerTests : TestBase
    {
        private IPlayer _player;
        private string _iPlayerHash;
        private Mock<IBallPit> _ballPitMock;
        private Mock<ILingoCardFactory> _cardFactoryMock;
        private Mock<ILingoCard> _cardMock;
        private Mock<IBall> _blueBallMock;
        private bool _useEvenNumbers;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _iPlayerHash = Solution.Current.GetFileHash(@"Lingo.Domain\Contracts\IPlayer.cs");
        }

        [SetUp]
        public void Setup()
        {
            _blueBallMock = new Mock<IBall>();
            _blueBallMock.SetupGet(b => b.Value).Returns(1);
            _blueBallMock.SetupGet(b => b.Type).Returns(BallType.Blue);

            _ballPitMock = new Mock<IBallPit>();
            _ballPitMock.Setup(pit => pit.GrabBall()).Returns(_blueBallMock.Object);

            _cardMock = new Mock<ILingoCard>();
            _cardFactoryMock = new Mock<ILingoCardFactory>();
            _cardFactoryMock.Setup(factory => factory.CreateNew(It.IsAny<bool>())).Returns(_cardMock.Object);
            _useEvenNumbers = RandomGenerator.NextBool();
            _player = new Player(Guid.NewGuid(), RandomGenerator.NextString(), _ballPitMock.Object, _cardFactoryMock.Object, _useEvenNumbers) as IPlayer;
        }

        [MonitoredTest("Should implement IPlayer")]
        public void _01_ShouldImplementIPlayer()
        {
            //Assert
            AssertThatInterfaceHasNotChanged();
            Assert.That(_player, Is.Not.Null);
        }

        [MonitoredTest("Constructor - Should initialize properly")]
        public void _02_Constructor_ShouldInitializeProperly()
        {
            AssertThatInterfaceHasNotChanged();

            //Arrange
            Guid userId = Guid.NewGuid();
            string userName = RandomGenerator.NextString();
            bool useEvenNumbers = RandomGenerator.NextBool();

            _cardFactoryMock.Invocations.Clear();
            _ballPitMock.Invocations.Clear();

            //Act
            _player = new Player(userId, userName, _ballPitMock.Object, _cardFactoryMock.Object, useEvenNumbers) as IPlayer;

            //Assert
            Assert.That(_player.Id, Is.EqualTo(userId), "The 'Id' property is not set correctly.");
            Assert.That(_player.Name, Is.EqualTo(userName), "The 'Name' property is not set correctly.");
            Assert.That(_player.Score, Is.EqualTo(0), "The 'Score' property should be zero.");
            Assert.That(_player.CanGrabBallFromBallPit, Is.False, "The 'CanGrabBallFromBallPit' should return false.");
            Assert.That(_player.BallPit, Is.SameAs(_ballPitMock.Object),
                "The 'BallPit' property is not set correctly. It should be the exact same object that was passed into the constructor.");

            _cardFactoryMock.Verify(factory => factory.CreateNew(useEvenNumbers), Times.Once,
                "The 'CreateNew' method of the card factory (that was passed into the constructor) was not called correctly.");
            Assert.That(_player.Card, Is.SameAs(_cardMock.Object),
                "The 'Card' property is not set correctly. " +
                "It should be the exact same object that is returned from a call to 'CreateNew' on the card factory that was passed into the constructor.");

            _ballPitMock.Verify(pit => pit.FillForLingoCard(_cardMock.Object), Times.Once,
                "After the constructor created the 'Card', the card should be used to fill the 'BallPit'.");
        }

        [MonitoredTest("RewardForSolvingPuzzle - Puzzle is finished but has score equal to zero - Should do nothing")]
        public void _03_RewardForSolvingPuzzle_PuzzleIsFinishedButScoreEqualToZero_ShouldDoNothing()
        {
            AssertThatInterfaceHasNotChanged();

            //Arrange
            int puzzleScore = 0;
            var puzzleMock = new Mock<IPuzzle>();
            puzzleMock.SetupGet(p => p.IsFinished).Returns(true);
            puzzleMock.SetupGet(p => p.Score).Returns(puzzleScore);

            //Act
            _player.RewardForSolvingPuzzle(puzzleMock.Object);

            //Assert
            Assert.That(_player.Score, Is.EqualTo(puzzleScore), "The 'Score' property should not have changed.");
            Assert.That(_player.CanGrabBallFromBallPit, Is.False, "The 'CanGrabBallFromBallPit' should return false.");
        }

        [MonitoredTest("RewardForSolvingPuzzle - Puzzle is not finished - Should throw InvalidOperationException")]
        public void _04_RewardForSolvingPuzzle_PuzzleIsNotFinished_ShouldThrowInvalidOperationException()
        {
            AssertThatInterfaceHasNotChanged();

            //Arrange
            var puzzleMock = new Mock<IPuzzle>();
            puzzleMock.SetupGet(p => p.IsFinished).Returns(false);
            puzzleMock.SetupGet(p => p.Score).Returns(0);

            //Act + Assert
            Assert.That(() => _player.RewardForSolvingPuzzle(puzzleMock.Object), Throws.InvalidOperationException);
        }

        [MonitoredTest("Scenario - Player solves a puzzle, grabs 2 blue balls and solves another puzzle")]
        public void _05_Scenario_PlayerSolvesAPuzzles_Grabs2BlueBalls_AndSolvesAnotherPuzzle()
        {
            AssertThatInterfaceHasNotChanged();

            //Arrange
            int puzzleScore = 25;
            var puzzleMock = new Mock<IPuzzle>();
            puzzleMock.SetupGet(p => p.IsFinished).Returns(true);
            puzzleMock.SetupGet(p => p.Score).Returns(puzzleScore);

            //Reward for first puzzle
            _player.RewardForSolvingPuzzle(puzzleMock.Object);

            Assert.That(_player.Score, Is.EqualTo(puzzleScore),
                $"The 'Score' should be {puzzleScore} after '{nameof(IPlayer.RewardForSolvingPuzzle)}' is called the first time.");
            Assert.That(_player.CanGrabBallFromBallPit, Is.True, $"The 'CanGrabBallFromBallPit' property should return true after '{nameof(IPlayer.RewardForSolvingPuzzle)}' is called the first time.");

            //Grab 2 balls
            IBall grabbedBall = _player.GrabBallFromBallPit();
            _ballPitMock.Verify(pit => pit.GrabBall(), Times.Once,
                $"After the first reward and when '{nameof(IPlayer.GrabBallFromBallPit)}' is called the first time, " +
                $"the '{nameof(IBallPit.GrabBall)}' method should have been called once.");
            Assert.That(grabbedBall, Is.SameAs(_blueBallMock.Object),
                $"The '{nameof(IPlayer.GrabBallFromBallPit)}' method should return the grabbed ball");
            _cardMock.Verify(card => card.CrossOutNumber(_blueBallMock.Object.Value), Times.Once,
                $"After the first reward and when '{nameof(IPlayer.GrabBallFromBallPit)}' is called the first time, " +
                $"the '{nameof(ILingoCard.CrossOutNumber)}' method should have been called on the card of the player");

            Assert.That(_player.CanGrabBallFromBallPit, Is.True,
                $"After the first reward and after '{nameof(IPlayer.GrabBallFromBallPit)}' is called one time, " +
                "the 'CanGrabBallFromBallPit' property should return true.");

            grabbedBall = _player.GrabBallFromBallPit();
            Assert.That(_player.CanGrabBallFromBallPit, Is.False,
                $"After the first reward and after '{nameof(IPlayer.GrabBallFromBallPit)}' is called a second time, " +
                "the 'CanGrabBallFromBallPit' property should return false.");
            _ballPitMock.Verify(pit => pit.GrabBall(), Times.Exactly(2),
                $"After the first reward and when '{nameof(IPlayer.GrabBallFromBallPit)}' is called a second, " +
                $"the '{nameof(IBallPit.GrabBall)}' method should have been called 2 times (in total).");
            Assert.That(grabbedBall, Is.SameAs(_blueBallMock.Object),
                $"The '{nameof(IPlayer.GrabBallFromBallPit)}' method should return the grabbed ball");
            _cardMock.Verify(card => card.CrossOutNumber(_blueBallMock.Object.Value), Times.Exactly(2),
                $"After the first reward and when '{nameof(IPlayer.GrabBallFromBallPit)}' is called a second time, " +
                $"the '{nameof(ILingoCard.CrossOutNumber)}' method should have been called 2 times (in total) on the card of the player");

            //Reward for another puzzle
            _player.RewardForSolvingPuzzle(puzzleMock.Object);

            Assert.That(_player.Score, Is.EqualTo(puzzleScore * 2),
                $"The 'Score' should be {puzzleScore * 2} after '{nameof(IPlayer.RewardForSolvingPuzzle)}' is called the second time.");
            Assert.That(_player.CanGrabBallFromBallPit, Is.True,
                "After the second reward, the 'CanGrabBallFromBallPit' property should return true.");
        }

        [MonitoredTest("Scenario - Player solves a puzzle and grabs a blue ball that creates a LINGO")]
        public void _06_Scenario_PlayerSolvesAPuzzles_GrabsABlueBallThatCreatesALingo()
        {
            AssertThatInterfaceHasNotChanged();

            //Arrange
            int puzzleScore = 25;
            var puzzleMock = new Mock<IPuzzle>();
            puzzleMock.SetupGet(p => p.IsFinished).Returns(true);
            puzzleMock.SetupGet(p => p.Score).Returns(puzzleScore);
            _cardMock.Setup(card => card.CrossOutNumber(It.IsAny<int>())).Callback(() =>
            {
                _cardMock.SetupGet(card => card.HasLingo).Returns(true);
            });

            //Reward for first puzzle
            _player.RewardForSolvingPuzzle(puzzleMock.Object);

            Assert.That(_player.Score, Is.EqualTo(puzzleScore),
                $"The 'Score' should be {puzzleScore} after '{nameof(IPlayer.RewardForSolvingPuzzle)}' is called the first time.");
            Assert.That(_player.CanGrabBallFromBallPit, Is.True, $"The 'CanGrabBallFromBallPit' property should return true after '{nameof(IPlayer.RewardForSolvingPuzzle)}' is called the first time.");

            //Grab the ball that should create a LINGO
            IBall grabbedBall = _player.GrabBallFromBallPit();
            _ballPitMock.Verify(pit => pit.GrabBall(), Times.Once,
                $"After the first reward and when '{nameof(IPlayer.GrabBallFromBallPit)}' is called the first time, " +
                $"the '{nameof(IBallPit.GrabBall)}' method should have been called once.");
            Assert.That(grabbedBall, Is.SameAs(_blueBallMock.Object),
                $"The '{nameof(IPlayer.GrabBallFromBallPit)}' method should return the grabbed ball");
            _cardMock.Verify(card => card.CrossOutNumber(_blueBallMock.Object.Value), Times.Once,
                $"After the first reward and when '{nameof(IPlayer.GrabBallFromBallPit)}' is called the first time, " +
                $"the '{nameof(ILingoCard.CrossOutNumber)}' method should have been called on the card of the player");

            Assert.That(_player.Score, Is.EqualTo(puzzleScore + 100),
                $"After the first reward and after '{nameof(IPlayer.GrabBallFromBallPit)}' is called, " +
                $"the 'Score' property should return {puzzleScore + 100} (score of the puzzle + 100 for the LINGO).");

            _cardFactoryMock.Verify(factory => factory.CreateNew(_useEvenNumbers), Times.Exactly(2),
                "When the player has a LINGO, a new card should be created using the card factory.");

            _ballPitMock.Verify(pit => pit.FillForLingoCard(_cardMock.Object), Times.Exactly(2),
                "When the player has a LINGO, the ball pit should be filled again using the newly created card.");

            Assert.That(_player.CanGrabBallFromBallPit, Is.False,
                $"After a LINGO the '{nameof(IPlayer.CanGrabBallFromBallPit)}' property should be false");
        }

        [MonitoredTest("Scenario - Player solves a puzzle and grabs a red ball")]
        public void _07_Scenario_PlayerSolvesAPuzzles_GrabsARedBall()
        {
            AssertThatInterfaceHasNotChanged();

            //Arrange
            int puzzleScore = 25;
            var puzzleMock = new Mock<IPuzzle>();
            puzzleMock.SetupGet(p => p.IsFinished).Returns(true);
            puzzleMock.SetupGet(p => p.Score).Returns(puzzleScore);

            var redBallMock = new Mock<IBall>();
            redBallMock.SetupGet(b => b.Value).Returns(0);
            redBallMock.SetupGet(b => b.Type).Returns(BallType.Red);

            _ballPitMock.Setup(pit => pit.GrabBall()).Returns(redBallMock.Object);

            //Reward for first puzzle
            _player.RewardForSolvingPuzzle(puzzleMock.Object);

            Assert.That(_player.Score, Is.EqualTo(puzzleScore),
                $"The 'Score' should be {puzzleScore} after '{nameof(IPlayer.RewardForSolvingPuzzle)}' is called the first time.");
            Assert.That(_player.CanGrabBallFromBallPit, Is.True, $"The 'CanGrabBallFromBallPit' property should return true after '{nameof(IPlayer.RewardForSolvingPuzzle)}' is called the first time.");

            //Grab red ball
            IBall grabbedBall = _player.GrabBallFromBallPit();
            _ballPitMock.Verify(pit => pit.GrabBall(), Times.Once,
                $"After the first reward and when '{nameof(IPlayer.GrabBallFromBallPit)}' is called the first time, " +
                $"the '{nameof(IBallPit.GrabBall)}' method should have been called once.");
            Assert.That(grabbedBall, Is.SameAs(redBallMock.Object),
                $"The '{nameof(IPlayer.GrabBallFromBallPit)}' method should return the grabbed ball");
            _cardMock.Verify(card => card.CrossOutNumber(_blueBallMock.Object.Value), Times.Never,
                $"After the first reward and when '{nameof(IPlayer.GrabBallFromBallPit)}' is called the first time, " +
                $"the '{nameof(ILingoCard.CrossOutNumber)}' method should NOT have been called on the card of the player");

            Assert.That(_player.CanGrabBallFromBallPit, Is.False,
                $"After the first reward and after '{nameof(IPlayer.GrabBallFromBallPit)}' is called one time (red ball), " +
                "the 'CanGrabBallFromBallPit' property should return false.");
        }

        private void AssertThatInterfaceHasNotChanged()
        {
            Assert.That(_iPlayerHash, Is.EqualTo("5A-E7-4F-C1-F5-7C-DA-5B-25-61-73-A5-E3-52-50-4B"),
                "The code of the IPlayer interface has changed. This is not allowed. Undo your changes in 'IPlayer.cs'");
        }
    }
}
