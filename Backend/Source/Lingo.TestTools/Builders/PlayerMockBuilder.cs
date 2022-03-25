using Lingo.Domain.Card.Contracts;
using Lingo.Domain.Contracts;
using Lingo.Domain.Pit.Contracts;
using Moq;

namespace Lingo.TestTools.Builders
{
    public class PlayerMockBuilder : MockBuilder<IPlayer>
    {
        private static readonly Random RandomGenerator = new Random();

        public Mock<ILingoCard> LingoCardMock { get; }

        public PlayerMockBuilder()
        {
            Mock.SetupGet(p => p.Id).Returns(Guid.NewGuid());
            Mock.SetupGet(p => p.Name).Returns(RandomGenerator.NextString());
            LingoCardMock = new LingoCardMockBuilder().Mock;
            Mock.SetupGet(p => p.Card).Returns(LingoCardMock.Object);
            Mock.SetupGet(p => p.Score).Returns(RandomGenerator.Next(0,100));
            Mock.SetupGet(p => p.CanGrabBallFromBallPit).Returns(false);
        }

        public PlayerMockBuilder WithCanGrabBallFromBallPit(bool canGrabBallFromBallPit)
        {
            Mock.SetupGet(p => p.CanGrabBallFromBallPit).Returns(canGrabBallFromBallPit);
            return this;
        }

        public PlayerMockBuilder WithNextBall(IBall nextBall, bool isLastBallThatCanBeGrabbed)
        {
            Mock.Setup(p => p.GrabBallFromBallPit()).Returns(() =>
            {
                Mock.SetupGet(p => p.CanGrabBallFromBallPit).Returns(!isLastBallThatCanBeGrabbed);
                return nextBall;
            });
            return this;
        }

        public PlayerMockBuilder WithId(Guid id)
        {
            Mock.SetupGet(p => p.Id).Returns(id);
            return this;
        }
    }
}