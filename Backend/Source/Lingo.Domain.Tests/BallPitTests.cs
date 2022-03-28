using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Lingo.Domain.Card.Contracts;
using Lingo.Domain.Pit;
using Lingo.Domain.Pit.Contracts;
using Lingo.TestTools;
using Lingo.TestTools.Builders;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Lingo", "BallPit", @"Lingo.Domain\Pit\BallPit.cs")]
    public class BallPitTests : TestBase
    {
        private string _iBallPitHash;
        private IBallPit _pit;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _iBallPitHash = Solution.Current.GetFileHash(@"Lingo.Domain\Pit\Contracts\IBallPit.cs");
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _pit = new BallPit() as IBallPit;
        }

        [MonitoredTest("Should implement IBallPit")]
        public void _01_ShouldImplementIBallPit()
        {
            AssertThatInterfaceIsNotChanged();
            Assert.That(_pit, Is.Not.Null);
        }

        [MonitoredTest("Constructor - Should initialize a private field that can contain the balls")]
        public void _02_Constructor_ShouldInitializeAPrivateFieldThatCanContainTheBalls()
        {
            AssertThatInterfaceIsNotChanged();
            IList<IBall> allBalls = GetAllBallsField();
            Assert.That(allBalls, Is.Not.Null, "After construction the field that holds all balls should not be NULL.");
            Assert.That(allBalls.Count(b => b.Type == BallType.Red), Is.Zero, "There shouldn't be any red balls in the pit after construction.");
            Assert.That(allBalls.Count(b => b.Type == BallType.Blue), Is.Zero, "There shouldn't be any blue balls in the pit after construction.");
        }

        [MonitoredTest("FillForLingoCard - No red or blue balls in the pit - Should add 3 red balls")]
        public void _03_FillForLingoCard_NoRedOrBlueBallsInThePit_ShouldAdd3RedBalls()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCardMockBuilder().Object;

            //Act
            _pit.FillForLingoCard(card);

            //Assert
            IList<IBall> allBalls = GetAllBallsField();
            Assert.That(allBalls.Count(b => b.Type == BallType.Red), Is.EqualTo(3));
        }

        [MonitoredTest("FillForLingoCard - No red or blue balls in the pit - Should add a blue ball for each non cross out number")]
        public void _04_FillForLingoCard_NoRedOrBlueBallsInThePit_ShouldAddABlueBallForEachNonCrossOutNumber()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCardMockBuilder().WithCrossedOutNumbers(8).Object;

            //Act
            _pit.FillForLingoCard(card);

            //Assert
            IList<IBall> allBalls = GetAllBallsField();

            Assert.That(allBalls.Count(b => b.Type == BallType.Blue), Is.EqualTo(17),
                "There should be 17 blue balls in the pit (25 numbers of which 8 are crossed out).");

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    ICardNumber cardNumber = card.CardNumbers[i, j];
                    if (!cardNumber.CrossedOut)
                    {
                        Assert.That(allBalls.Count(b => b.Type == BallType.Blue && b.Value == cardNumber.Value),
                            Is.EqualTo(1),
                            $"The card had a the number '{cardNumber.Value}' not crossed out, " +
                            "but no blue ball with that value is found in the pit.");
                    }
                }
            }
        }

        [MonitoredTest("GrabBall - Should randomly pick a ball")]
        public void _05_GrabBall_ShouldRandomlyPickABall()
        {
            AssertThatInterfaceIsNotChanged();

            IList<IBall> testBalls = new List<IBall>();
            for (int i = 0; i < 17; i++)
            {
                testBalls.Add(new BallMockBuilder().Object);
            }

            IBall previouslyGrabbedBall = null;
            int numberOfRuns = 20;
            int numberOfTimesTheSameBallWasGrabbed = 0;

            for (int i = 0; i < numberOfRuns; i++)
            {
                //Arrange
                _pit = new BallPit() as IBallPit;
                IList<IBall> allBallsOfPit = GetAllBallsField();
                foreach (IBall ball in testBalls)
                {
                    allBallsOfPit.Add(ball);
                }

                //Act
                IBall grabbedBall = _pit.GrabBall();

                //Assert
                Assert.That(allBallsOfPit.Count, Is.EqualTo(16), "The number of balls in the pit should have decremented by one.");
                Assert.That(allBallsOfPit, Has.None.SameAs(grabbedBall), "The grabbed ball can still be found in the pit.");

                if (previouslyGrabbedBall != null && previouslyGrabbedBall.Equals(grabbedBall))
                {
                    numberOfTimesTheSameBallWasGrabbed++;
                }

                previouslyGrabbedBall = grabbedBall;
            }

            Assert.That(numberOfTimesTheSameBallWasGrabbed, Is.LessThan(numberOfRuns / 2),
                "The ball grabbing does not seem to be random enough. Often the same ball is grabbed first.");

        }

        [MonitoredTest("GrabBall - Red ball - Should be returned to the pit")]
        public void _06_GrabBall_RedBall_ShouldBeReturnedToThePit()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            IList<IBall> redBalls = new List<IBall>();
            for (int i = 0; i < 3; i++)
            {
                redBalls.Add(new BallMockBuilder().AsRed().Object);
            }
            _pit = new BallPit() as IBallPit;
            IList<IBall> allBallsOfPit = GetAllBallsField();
            foreach (IBall ball in redBalls)
            {
                allBallsOfPit.Add(ball);
            }

            //Act
            _pit.GrabBall();

            //Assert
            Assert.That(allBallsOfPit.Count, Is.EqualTo(3),
                "The number of balls in the pit should not have been decremented when a red ball is grabbed.");

        }

        [MonitoredTest("FillForLingoCard - Already some balls in the pit - Should remove red and blue balls before filling the pit")]
        public void _07_FillForLingoCard_AlreadySomeBallsInThePit_ShouldRemoveRedAndBlueBallsBeforeFillingThePit()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCardMockBuilder().WithCrossedOutNumbers(8).Object;
            _pit.FillForLingoCard(card);
            _pit.GrabBall();
            _pit.GrabBall();

            //Act
            _pit.FillForLingoCard(card); //Fill once more.

            //Assert
            IList<IBall> allBalls = GetAllBallsField();

            Assert.That(allBalls.Count(b => b.Type == BallType.Red), Is.EqualTo(3),
                "There should be 3 red balls in the pit");

            Assert.That(allBalls.Count(b => b.Type == BallType.Blue), Is.EqualTo(17),
                "There should be 17 blue balls in the pit (25 numbers of which 8 are crossed out).");
        }

        private IList<IBall> GetAllBallsField()
        {
            var field = typeof(BallPit).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(f => f.FieldType.IsAssignableTo(typeof(IList<IBall>)));

            Assert.That(field, Is.Not.Null,
                "There should be a private field that can hold a list of balls. The field should be assignable to a variable of type 'IList<IBall>'.");

            return field.GetValue(_pit) as IList<IBall>;
        }

        private void AssertThatInterfaceIsNotChanged()
        {
            Assert.That(_iBallPitHash, Is.EqualTo("33-C1-FB-55-B0-B4-AD-03-9F-D6-98-E4-C1-98-85-65"),
                "The code of the IBallPit interface has changed. This is not allowed. Undo your changes in 'IBallPit.cs'");
        }
    }
}