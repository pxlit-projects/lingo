using Lingo.Domain.Pit;
using Lingo.Domain.Pit.Contracts;

namespace Lingo.TestTools.Builders;

public class BallMockBuilder : MockBuilder<IBall>
{
    private static readonly Random RandomGenerator = new Random();

    public BallMockBuilder()
    {
        int value = RandomGenerator.Next(1, 71);
        Mock.SetupGet(b => b.Value).Returns(value);
        Mock.SetupGet(b => b.Type).Returns(BallType.Blue);
    }

    public BallMockBuilder AsRed()
    {
        Mock.SetupGet(b => b.Value).Returns(0);
        Mock.SetupGet(b => b.Type).Returns(BallType.Red);
        return this;
    }
}