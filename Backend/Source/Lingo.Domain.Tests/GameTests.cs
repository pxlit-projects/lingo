using System;
using System.Collections.Generic;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Lingo.Domain.Contracts;
using Lingo.Domain.Pit.Contracts;
using Lingo.Domain.Puzzle.Contracts;
using Lingo.TestTools;
using Lingo.TestTools.Builders;
using Moq;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Lingo", "Game", @"Lingo.Domain\Game.cs;Lingo.Domain\GameSettings.cs")]
    public class GameTests : TestBase
    {
        private PlayerMockBuilder _player1MockBuilder;
        private PlayerMockBuilder _player2MockBuilder;
        private List<IPuzzle> _puzzles;
        private IGame _game;
        private string _iGameHash;
        private WordPuzzleMockBuilder _puzzle1MockBuilder;
        private WordPuzzleMockBuilder _puzzle2MockBuilder;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _iGameHash = Solution.Current.GetFileHash(@"Lingo.Domain\Contracts\IGame.cs");
        }

        [SetUp]
        public void Setup()
        {
            _player1MockBuilder = new PlayerMockBuilder();
            _player2MockBuilder = new PlayerMockBuilder();
            _puzzle1MockBuilder = new WordPuzzleMockBuilder();
            _puzzle2MockBuilder = new WordPuzzleMockBuilder();
            _puzzles = new List<IPuzzle>
            {
                _puzzle1MockBuilder.Object,
                _puzzle2MockBuilder.Object
            };

            _game = new Game(_player1MockBuilder.Object, _player2MockBuilder.Object, _puzzles) as IGame;
        }

        [MonitoredTest("Should implement IGame")]
        public void _01_ShouldImplementIGame()
        {
            //Assert
            AssertThatInterfaceHasNotChanged();
            Assert.That(_game, Is.Not.Null);
        }

        [MonitoredTest("Constructor - Should initialize properly")]
        public void _02_Constructor_ShouldInitializeProperly()
        {
            AssertThatInterfaceHasNotChanged();

            //Assert
            IPlayer player1 = _player1MockBuilder.Object;
            IPlayer player2 = _player2MockBuilder.Object;
            Assert.That(_game.Id, Is.Not.EqualTo(Guid.Empty), "The 'Id' property is not set correctly. It should be a non-empty Guid.");
            Assert.That(_game.Player1, Is.SameAs(player1), "The 'Player1' property is not set correctly.");
            Assert.That(_game.Player2, Is.SameAs(player2), "The 'Player2' property is not set correctly.");
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(player1.Id), "The 'PlayerToPlayId' should be the id of player1 after construction.");
            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzles[0]),
                "The 'CurrentPuzzle' property should be the first puzzle that was given to the constructor.");
            Assert.That(_game.Finished, Is.False, "The 'Finished' property should be false after construction.");
        }

        [MonitoredTest("SubmitAnswer - It's not the players turn - Should throw ApplicationException")]
        public void _03_SubmitAnswer_ItsNotThePlayersTurn_ShouldThrowApplicationException()
        {
            AssertThatInterfaceHasNotChanged();
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_game.Player1.Id), "After construction it should be player1's turn.");
            Assert.That(() => _game.SubmitAnswer(_game.Player2.Id, "WORD"),
                Throws.TypeOf<ApplicationException>().With.Message.Contains("beurt").IgnoreCase);
            _puzzle1MockBuilder.Mock.Verify(p => p.SubmitAnswer(It.IsAny<string>()), Times.Never,
                "The 'SubmitAnswer' method of the current puzzle should not have been called.");
        }

        [MonitoredTest("SubmitAnswer - Puzzle is finished - Should throw ApplicationException")]
        public void _04_SubmitAnswer_PuzzleIsFinished_ShouldThrowApplicationException()
        {
            AssertThatInterfaceHasNotChanged();
            AssertFirstPuzzleIsCurrentPuzzle();

            _puzzle1MockBuilder.WithIsFinished(true);

            Assert.That(() => _game.SubmitAnswer(_game.PlayerToPlayId, "WORD"),
                Throws.TypeOf<ApplicationException>().With.Message.Contains("beëindigd").IgnoreCase);
            _puzzle1MockBuilder.Mock.Verify(p => p.SubmitAnswer(It.IsAny<string>()), Times.Never,
                "The 'SubmitAnswer' method of the current puzzle should not have been called.");
        }

        [MonitoredTest("SubmitAnswer - Valid submission - Should keep turn")]
        public void _05_SubmitAnswer_ValidSubmission_ShouldKeepTurn()
        {
            AssertThatInterfaceHasNotChanged();
            AssertFirstPuzzleIsCurrentPuzzle();

            //Arrange
            string answer = "WORD";
            Guid currentPlayerId = _game.PlayerToPlayId;

            //Act
            SubmissionResult result = _game.SubmitAnswer(currentPlayerId, answer);

            //Assert
            Assert.That(result.LostTurn, Is.False, "The returned result indicates that the player lost its turn.");
            _puzzle1MockBuilder.Mock.Verify(p => p.SubmitAnswer(answer), Times.Once,
                "The 'SubmitAnswer' method of the current puzzle was not called correctly.");
            _puzzle1MockBuilder.Mock.Verify(p => p.RevealPart(), Times.Never,
                "The 'RevealPart' method of the current puzzle should not have been called.");
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(currentPlayerId), "The PlayerToPlayId should not have been changed.");

            _player1MockBuilder.Mock.Verify(p => p.RewardForSolvingPuzzle(It.IsAny<IPuzzle>()), Times.Never,
                "Player 1 should not have been rewarded for solving the puzzle. The puzzle is not finished yet.");

            _player2MockBuilder.Mock.Verify(p => p.RewardForSolvingPuzzle(It.IsAny<IPuzzle>()), Times.Never,
                "Player 2 should not have been rewarded for solving the puzzle. The puzzle is not finished yet.");

            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle1MockBuilder.Object), "The CurrentPuzzle of the game should not have been changed.");
        }

        [MonitoredTest("SubmitAnswer - Submission causes losing turn - Should reveal a part of the puzzle and give turn to opponent")]
        public void _06_SubmitAnswer_SubmissionCausesLosingTurn_ShouldRevealAPartOfThePuzzleAndGiveTurnToOpponent()
        {
            AssertThatInterfaceHasNotChanged();
            AssertFirstPuzzleIsCurrentPuzzle();

            //Arrange
            string answer = "WORD";
            Guid currentPlayerId = _game.PlayerToPlayId;

            SubmissionResult loseTurnResult = SubmissionResult.CreateLoseTurnResult("Some reason for losing turn");
            _puzzle1MockBuilder.WithSubmissionResult(loseTurnResult);

            //Act
            SubmissionResult result = _game.SubmitAnswer(currentPlayerId, answer);

            //Assert
            _puzzle1MockBuilder.Mock.Verify(p => p.SubmitAnswer(answer), Times.Once,
                "The 'SubmitAnswer' method of the current puzzle was not called correctly.");
            Assert.That(result, Is.SameAs(loseTurnResult), "The returned result should be the exact same object returned by the current puzzle.");

            _puzzle1MockBuilder.Mock.Verify(p => p.RevealPart(), Times.Once,
                "The 'RevealPart' method of the current puzzle should have been called.");
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_player2MockBuilder.Object.Id), "The PlayerToPlayId should have been changed to the id of player 2.");

            _player1MockBuilder.Mock.Verify(p => p.RewardForSolvingPuzzle(It.IsAny<IPuzzle>()), Times.Never,
                "Player 1 should not have been rewarded for solving the puzzle. The puzzle is not finished yet.");

            _player2MockBuilder.Mock.Verify(p => p.RewardForSolvingPuzzle(It.IsAny<IPuzzle>()), Times.Never,
                "Player 2 should not have been rewarded for solving the puzzle. The puzzle is not finished yet.");

            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle1MockBuilder.Object), "The CurrentPuzzle of the game should not have been changed.");
        }

        [MonitoredTest("SubmitAnswer - Submission causes puzzle to finish - Should reward player for solving the puzzle")]
        public void _07_SubmitAnswer_SubmissionCausesPuzzleToFinish_ShouldRewardPlayerForSolvingThePuzzle()
        {
            AssertThatInterfaceHasNotChanged();
            AssertFirstPuzzleIsCurrentPuzzle();

            //Arrange
            string answer = "WORD";
            Guid currentPlayerId = _game.PlayerToPlayId;

            _puzzle1MockBuilder.WithSubmissionResult(SubmissionResult.CreateKeepTurnResult(),
                causesThePuzzleToFinish: true);
            _player1MockBuilder.WithCanGrabBallFromBallPit(true);

            //Act
            SubmissionResult result = _game.SubmitAnswer(currentPlayerId, answer);

            //Assert
            _puzzle1MockBuilder.Mock.Verify(p => p.SubmitAnswer(answer), Times.Once,
                "The 'SubmitAnswer' method of the current puzzle was not called correctly.");
            Assert.That(result.LostTurn, Is.False, "The returned result should not indicate that the player lost its turn.");

            _puzzle1MockBuilder.Mock.Verify(p => p.RevealPart(), Times.Never,
                "The 'RevealPart' method of the current puzzle should not have been called.");
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(currentPlayerId), "The PlayerToPlayId should not have been changed.");

            _player1MockBuilder.Mock.Verify(p => p.RewardForSolvingPuzzle(_puzzle1MockBuilder.Object), Times.Once,
                "The 'RewardForSolvingPuzzle' method was not called correctly for player 1.");

            _player2MockBuilder.Mock.Verify(p => p.RewardForSolvingPuzzle(It.IsAny<IPuzzle>()), Times.Never,
                "Player 2 should not have been rewarded for solving the puzzle.");

            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle1MockBuilder.Object),
                "The CurrentPuzzle of the game should not have been changed (Player 1 has to grab balls from the ball pit first.");
        }

        [MonitoredTest("GrabBallFromBallPit - It's not the players turn - Should throw ApplicationException")]
        public void _08_GrabBallFromBallPit_ItsNotThePlayersTurn_ShouldThrowApplicationException()
        {
            AssertThatInterfaceHasNotChanged();
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_game.Player1.Id), "After construction it should be player1's turn.");

            Assert.That(() => _game.GrabBallFromBallPit(_game.Player2.Id),
                Throws.TypeOf<ApplicationException>().With.Message.Contains("beurt").IgnoreCase);

            _player1MockBuilder.Mock.Verify(p => p.GrabBallFromBallPit(), Times.Never,
                "The 'GrabBallFromBallPit' method should not have been called for player 1.");

            _player2MockBuilder.Mock.Verify(p => p.GrabBallFromBallPit(), Times.Never,
                "The 'GrabBallFromBallPit' method should not have been called for player 2.");

            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle1MockBuilder.Object), "The CurrentPuzzle of the game should not have been changed.");
        }

        [MonitoredTest("GrabBallFromBallPit - Player is not allowed to grab a ball - Should throw ApplicationException")]
        public void _09_GrabBallFromBallPit_PlayerIsNotAllowedToGrabABall_ShouldThrowApplicationException()
        {
            AssertThatInterfaceHasNotChanged();

            _player1MockBuilder.WithCanGrabBallFromBallPit(false);

            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_game.Player1.Id), "After construction it should be player1's turn.");

            Assert.That(() => _game.GrabBallFromBallPit(_game.Player1.Id),
                Throws.TypeOf<ApplicationException>().With.Message.Contains("trekken").IgnoreCase);

            _player1MockBuilder.Mock.VerifyGet(p => p.CanGrabBallFromBallPit, Times.Once,
                "The 'CanGrabBallFromBallPit' method should have been called for player 1.");

            _player1MockBuilder.Mock.Verify(p => p.GrabBallFromBallPit(), Times.Never,
                "The 'GrabBallFromBallPit' method should not have been called for player 1.");

            _player2MockBuilder.Mock.Verify(p => p.GrabBallFromBallPit(), Times.Never,
                "The 'GrabBallFromBallPit' method should not have been called for player 2.");

            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle1MockBuilder.Object), "The CurrentPuzzle of the game should not have been changed.");
        }

        [MonitoredTest("GrabBallFromBallPit - First ball of 2 is blue - Should return grabbed ball")]
        public void _10_GrabBallFromBallPit_FirstBallOf2IsBlue_ShouldReturnGrabbedBall()
        {
            AssertThatInterfaceHasNotChanged();
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_game.Player1.Id), "After construction it should be player1's turn.");

            //Arrange
            IBall nextBall = new BallMockBuilder().Object;
            _player1MockBuilder.WithCanGrabBallFromBallPit(true).WithNextBall(nextBall, isLastBallThatCanBeGrabbed: false);

            //Act
            IBall ball = _game.GrabBallFromBallPit(_game.Player1.Id);

            Assert.That(ball, Is.SameAs(nextBall),
                "The returned ball should be the exact same object that is returned by the 'GrabBallFromBallPit' method of player 1");

            _player1MockBuilder.Mock.VerifyGet(p => p.CanGrabBallFromBallPit, Times.AtLeastOnce,
                "The 'CanGrabBallFromBallPit' method should have been called for player 1.");

            _player2MockBuilder.Mock.Verify(p => p.GrabBallFromBallPit(), Times.Never,
                "The 'GrabBallFromBallPit' method should not have been called for player 2.");

            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle1MockBuilder.Object), "The CurrentPuzzle of the game should not have been changed.");
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_game.Player1.Id), "After grabbing a blue ball it should still be player1's turn.");
        }

        [MonitoredTest("GrabBallFromBallPit - Second ball of 2 is blue - Should move game to next puzzle")]
        public void _11_GrabBallFromBallPit_SecondBallOf2IsBlue_ShouldMoveGameToNextPuzzle()
        {
            AssertThatInterfaceHasNotChanged();
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_game.Player1.Id), "After construction it should be player1's turn.");

            //Arrange
            IBall nextBall = new BallMockBuilder().Object;
            _player1MockBuilder.WithCanGrabBallFromBallPit(true).WithNextBall(nextBall, isLastBallThatCanBeGrabbed: true);

            //Act
            IBall ball = _game.GrabBallFromBallPit(_game.Player1.Id);

            Assert.That(ball, Is.SameAs(nextBall),
                "The returned ball should be the exact same object that is returned by the 'GrabBallFromBallPit' method of player 1");

            _player1MockBuilder.Mock.VerifyGet(p => p.CanGrabBallFromBallPit, Times.AtLeast(2),
                "The 'CanGrabBallFromBallPit' method should have been called at least 2 times for player 1. " +
                "One time before the ball grab and one time after the ball grab to check if the game needs to move to the next puzzle.");

            _player2MockBuilder.Mock.Verify(p => p.GrabBallFromBallPit(), Times.Never,
                "The 'GrabBallFromBallPit' method should not have been called for player 2.");

            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle2MockBuilder.Object), "The CurrentPuzzle of the game should have been changed to the next puzzle.");
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_game.Player1.Id), "After grabbing a blue ball it should still be player1's turn.");
        }

        [MonitoredTest("GrabBallFromBallPit - Red ball - Should give turn to opponent and move game to next puzzle")]
        public void _12_GrabBallFromBallPit_RedBall_ShouldGiveTurnToOpponentAndMoveGameToNextPuzzle()
        {
            AssertThatInterfaceHasNotChanged();
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_game.Player1.Id), "After construction it should be player1's turn.");

            //Arrange
            IBall nextBall = new BallMockBuilder().AsRed().Object;
            _player1MockBuilder.WithCanGrabBallFromBallPit(true).WithNextBall(nextBall, isLastBallThatCanBeGrabbed: true);

            //Act
            IBall ball = _game.GrabBallFromBallPit(_game.Player1.Id);

            Assert.That(ball, Is.SameAs(nextBall),
                "The returned ball should be the exact same object that is returned by the 'GrabBallFromBallPit' method of player 1");

            _player1MockBuilder.Mock.VerifyGet(p => p.CanGrabBallFromBallPit, Times.AtLeast(2),
                "The 'CanGrabBallFromBallPit' method should have been called at least 2 times for player 1. " +
                "One time before the ball grab and one time after the ball grab to check if the game needs to move to the next puzzle.");

            _player2MockBuilder.Mock.Verify(p => p.GrabBallFromBallPit(), Times.Never,
                "The 'GrabBallFromBallPit' method should not have been called for player 2.");

            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle2MockBuilder.Object), "The CurrentPuzzle of the game should have been changed to the next puzzle.");
            Assert.That(_game.PlayerToPlayId, Is.EqualTo(_game.Player2.Id), "After grabbing a red ball it should be player2's turn.");
        }

        [MonitoredTest("Finished - Last puzzle is finished and none of the players can grab a ball - Should return true")]
        public void _13_Finished_LastPuzzleIsFinishedAndNoneOfThePlayersCanGrabABall_ShouldReturnTrue()
        {
            AssertThatInterfaceHasNotChanged();

            //Arrange
            IBall nextBall = new BallMockBuilder().Object;
            _puzzle1MockBuilder.WithSubmissionResult(SubmissionResult.CreateKeepTurnResult(), causesThePuzzleToFinish: true);
            _puzzle2MockBuilder.WithSubmissionResult(SubmissionResult.CreateKeepTurnResult(), causesThePuzzleToFinish: true);
            _player1MockBuilder.WithCanGrabBallFromBallPit(true).WithNextBall(nextBall, isLastBallThatCanBeGrabbed: true);
            _player2MockBuilder.WithCanGrabBallFromBallPit(false);

            _game.SubmitAnswer(_game.PlayerToPlayId, "WORD"); //should finish puzzle 1
            _game.GrabBallFromBallPit(_game.PlayerToPlayId); //should move to puzzle 2
            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle2MockBuilder.Object),
                "The simulation of a full game play failed. " +
                "The CurrentPuzzle did not moved to the second puzzle after puzzle 1 is finished and the last ball is grabbed by player 1.");
            _game.SubmitAnswer(_game.PlayerToPlayId, "WORD"); //should finish puzzle 2

            //Act
            bool isFinished = _game.Finished;

            Assert.That(isFinished, Is.True);
        }

        [MonitoredTest("Finished - Not all puzzles are finished - Should return false")]
        public void _14_Finished_NotAllPuzzlesFinished_ShouldReturnFalse()
        {
            AssertThatInterfaceHasNotChanged();

            //Arrange
            IBall nextBall = new BallMockBuilder().Object;
            _puzzle1MockBuilder.WithSubmissionResult(SubmissionResult.CreateKeepTurnResult(), causesThePuzzleToFinish: true);
            _puzzle2MockBuilder.WithIsFinished(false);
            _player1MockBuilder.WithCanGrabBallFromBallPit(true).WithNextBall(nextBall, isLastBallThatCanBeGrabbed: true);
            _player2MockBuilder.WithCanGrabBallFromBallPit(false);

            _game.SubmitAnswer(_game.PlayerToPlayId, "WORD"); //should finish puzzle 1
            _game.GrabBallFromBallPit(_game.PlayerToPlayId); //should move to puzzle 2
            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle2MockBuilder.Object),
                "The simulation of a full game play failed. " +
                "The CurrentPuzzle did not moved to the second puzzle after puzzle 1 is finished and the last ball is grabbed by player 1.");

            //Act
            bool isFinished = _game.Finished;

            Assert.That(isFinished, Is.False);
        }

        [MonitoredTest("Finished - All puzzles are finished but a player can grab a ball - Should return false")]
        public void _15_Finished_AllPuzzlesFinishedButAPlayerCanGrabABall_ShouldReturnFalse()
        {
            AssertThatInterfaceHasNotChanged();

            //Arrange
            IBall nextBall = new BallMockBuilder().Object;
            _puzzle1MockBuilder.WithSubmissionResult(SubmissionResult.CreateKeepTurnResult(), causesThePuzzleToFinish: true);
            _puzzle2MockBuilder.WithSubmissionResult(SubmissionResult.CreateKeepTurnResult(), causesThePuzzleToFinish: true);
            _player1MockBuilder.WithCanGrabBallFromBallPit(true).WithNextBall(nextBall, isLastBallThatCanBeGrabbed: true);
            _player2MockBuilder.WithCanGrabBallFromBallPit(false);

            _game.SubmitAnswer(_game.PlayerToPlayId, "WORD"); //should finish puzzle 1
            _game.GrabBallFromBallPit(_game.PlayerToPlayId); //should move to puzzle 2
            Assert.That(_game.CurrentPuzzle, Is.SameAs(_puzzle2MockBuilder.Object),
                "The simulation of a full game play failed. " +
                "The CurrentPuzzle did not moved to the second puzzle after puzzle 1 is finished and the last ball is grabbed by player 1.");
            _game.SubmitAnswer(_game.PlayerToPlayId, "WORD"); //should finish puzzle 2
            _player1MockBuilder.WithCanGrabBallFromBallPit(true);

            //Act
            bool isFinished = _game.Finished;

            Assert.That(isFinished, Is.False);
        }

        private void AssertThatInterfaceHasNotChanged()
        {
            Assert.That(_iGameHash, Is.EqualTo("69-23-68-F5-E7-F8-AB-AE-1F-2B-3F-53-F7-44-76-BE"),
                "The code of the IGame interface has changed. This is not allowed. Undo your changes in 'IGame.cs'");
        }

        private void AssertFirstPuzzleIsCurrentPuzzle()
        {
            IWordPuzzle puzzle1 = _puzzle1MockBuilder.Object;
            Assert.That(_game.CurrentPuzzle.Id, Is.EqualTo(puzzle1.Id), "The CurrentPuzzle is not set to the first puzzle passed into the constructor.");
        }
    }
}
