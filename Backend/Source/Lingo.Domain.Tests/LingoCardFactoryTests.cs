using Guts.Client.Core;
using Lingo.Domain.Card;
using Lingo.Domain.Card.Contracts;
using Lingo.TestTools;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    public class LingoCardFactoryTests : TestBase
    {
        private ILingoCardFactory _factory;

        [SetUp]
        public void BeforeEachTest()
        {
            _factory = new LingoCardFactory() as ILingoCardFactory;
        }

        [MonitoredTest("Should implement ILingoCardFactory")]
        public void _01_ShouldImplementILingoCardFactory()
        {
            //Assert
            Assert.That(_factory, Is.Not.Null);
        }

        [MonitoredTest("CreateNew - Should create a card with valid numbers")]
        public void _02_CreateNew_ShouldCreateACardWithValidNumbers()
        {
            //Arrange
            bool useEvenNumbers = RandomGenerator.NextBool();

            //Act
            ILingoCard card = _factory.CreateNew(useEvenNumbers);

            //Assert
            Assert.That(card, Is.Not.Null);
            Assert.That(card.CardNumbers, Is.Not.Null, "The 'CardNumbers' property of the created card should not be null.");
            for (int i = 0; i < card.CardNumbers.GetLength(0); i++)
            {
                for (int j = 0; j < card.CardNumbers.GetLength(1); j++)
                {
                    Assert.That(card.CardNumbers[i,j], Is.Not.Null, "Each cell of the 'CardNumbers' of the created card should not be null.");
                    if (useEvenNumbers)
                    {
                        Assert.That(card.CardNumbers[i, j].Value % 2 == 0, Is.True,
                            $"When 'useEvenNumbers' is true, all numbers on the card should be even, but the card contains the number '{card.CardNumbers[i, j].Value}'.");
                    }
                    else
                    {
                        Assert.That(card.CardNumbers[i, j].Value % 2 != 0, Is.True,
                            $"When 'useEvenNumbers' is false, all numbers on the card should be odd, but the card contains the number '{card.CardNumbers[i, j].Value}'.");
                    }
                }
            }
        }
    }
}
