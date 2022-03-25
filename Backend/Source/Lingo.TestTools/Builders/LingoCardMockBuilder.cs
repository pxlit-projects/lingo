using Lingo.Domain.Card.Contracts;

namespace Lingo.TestTools.Builders
{
    public class LingoCardMockBuilder : MockBuilder<ILingoCard>
    {
        private static readonly Random RandomGenerator = new Random();
        private readonly ICardNumber[,] _cardNumbers;

        public LingoCardMockBuilder()
        {
            _cardNumbers = new ICardNumber[5, 5];
            int value = 2;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    _cardNumbers[i, j] = new CardNumberMockBuilder().WithValue(value).Object;
                    value += 2;
                }
            }

            Mock.SetupGet(c => c.HasLingo).Returns(false);
            Mock.SetupGet(p => p.CardNumbers).Returns(_cardNumbers);
        }

        public LingoCardMockBuilder WithCrossedOutNumbers(int numberOfCrossedOutNumbers)
        {
            int startCellIndex = RandomGenerator.Next(0, 25 - numberOfCrossedOutNumbers);
            
            for (int cellIndex = startCellIndex; cellIndex < startCellIndex + numberOfCrossedOutNumbers; cellIndex++)
            {
                int i = cellIndex / 5;
                int j = cellIndex % 5;

                _cardNumbers[i, j] = new CardNumberMockBuilder()
                    .WithValue(_cardNumbers[i, j].Value)
                    .WithCrossedOut(true)
                    .Object;
            }

            return this;
        }
    }
}
