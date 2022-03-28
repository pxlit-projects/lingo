using System;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Lingo.Domain.Card;
using Lingo.Domain.Card.Contracts;
using Lingo.TestTools;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Lingo", "CardNumber", @"Lingo.Domain\Card\CardNumber.cs")]
    public class CardNumberTests : TestBase
    {
        private string _iCardNumberCodeHash;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _iCardNumberCodeHash = Solution.Current.GetFileHash(@"Lingo.Domain\Card\Contracts\ICardNumber.cs");
        }

        [MonitoredTest("Should have a constructor that accepts a number")]
        public void _01_ShouldHaveAConstructorThatAcceptsANumber()
        {
            AssertThatInterfaceIsNotChanged();

            int value = RandomGenerator.Next(1, 75);
            ICardNumber number = ConstructCardNumber(value);
            Assert.That(number.Value, Is.EqualTo(value),
                "After construction the 'Value' property should return the value passed into the constructor.");
            Assert.That(number.CrossedOut, Is.False,
                "After construction the 'CrossedOut' property should be false.");
        }

        private ICardNumber ConstructCardNumber(int value)
        {
            var cardNumberType = typeof(CardNumber);

            int numberOfConstructors = cardNumberType.GetConstructors().Length;
            Assert.That(numberOfConstructors, Is.EqualTo(1), "There should be exactly one constructor. No more, no less.");

            CardNumber number = null;
            try
            {
                number = (CardNumber)Activator.CreateInstance(typeof(CardNumber), value);
            }
            catch (MissingMethodException)
            {
                Assert.Fail("Cannot find a public constructor that has a parameter of type integer");
            }

            ICardNumber numberAsInterfaceType = number as ICardNumber;
            Assert.That(numberAsInterfaceType, Is.Not.Null, "'CardNumber' must implement 'ICardNumber'.");

            return numberAsInterfaceType;
        }


        private void AssertThatInterfaceIsNotChanged()
        {
            Assert.That(_iCardNumberCodeHash, Is.EqualTo("21-5A-70-7A-C1-8B-C5-D8-8B-F2-18-87-C4-A5-81-07"),
                "The code of the ICardNumber interface has changed. This is not allowed. Undo your changes in 'ICardNumber.cs'");
        }
    }
}