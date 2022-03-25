using Lingo.Domain.Card.Contracts;

namespace Lingo.TestTools.Builders;

public class CardNumberMockBuilder : MockBuilder<ICardNumber>
{
    private static readonly Random RandomGenerator = new Random();

    public CardNumberMockBuilder()
    {
        int value = RandomGenerator.Next(1, 71);
        Mock.SetupGet(cn => cn.Value).Returns(value);
        Mock.SetupProperty(cn => cn.CrossedOut, false);
    }

    public CardNumberMockBuilder WithValue(int value)
    {
        Mock.SetupGet(cn => cn.Value).Returns(value);
        return this;
    }

    public CardNumberMockBuilder WithCrossedOut(bool isCrossedOut)
    {
        Mock.SetupProperty(cn => cn.CrossedOut, isCrossedOut);
        return this;
    }
}