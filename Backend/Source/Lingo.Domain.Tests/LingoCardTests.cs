using System.Collections.Generic;
using System.Linq;
using System.Text;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Lingo.Domain.Card;
using Lingo.Domain.Card.Contracts;
using Lingo.TestTools;
using NUnit.Framework;

namespace Lingo.Domain.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Lingo", "LingoCard", @"Lingo.Domain\Card\LingoCard.cs")]
    public class LingoCardTests : TestBase
    {
        private string _iLingoCardHash;

        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            _iLingoCardHash = Solution.Current.GetFileHash(@"Lingo.Domain\Card\Contracts\ILingoCard.cs");
        }

        [MonitoredTest("Should implement ILingoCard")]
        public void _01_ShouldImplementILingoCard()
        {
            ILingoCard card = new LingoCard(true) as ILingoCard;

            //Assert
            AssertThatInterfaceIsNotChanged();
            Assert.That(card, Is.Not.Null);
        }

        [MonitoredTest("Constructor - Even numbers - Should randomly fill card with even numbers")]
        public void _02_Constructor_EvenNumbers_ShouldRandomlyFillCardWithEvenNumbers()
        {
            AssertThatInterfaceIsNotChanged();

            TestNumberGeneration(true);
        }

        [MonitoredTest("Constructor - Odd Numbers - Should randomly fill card with odd numbers")]
        public void _03_Constructor_OddNumbers_ShouldRandomlyFillCardWithOddNumbers()
        {
            AssertThatInterfaceIsNotChanged();

            TestNumberGeneration(false);
        }

        [MonitoredTest("HasLingo - Vertical Lingo - Should return true")]
        public void _04_HasLingo_VerticalLingo_ShouldReturnTrue()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCard(RandomGenerator.NextBool()) as ILingoCard;

            UncrossWholeCard(card);

            //cross out random column
            int lingoColumnIndex = RandomGenerator.Next(0, 5);
            for (int i = 0; i < 5; i++)
            {
                card.CardNumbers[i, lingoColumnIndex].CrossedOut = true;
            }

            //Act
            bool hasLingo = card.HasLingo;

            //Assert
            Assert.That(hasLingo, Is.True, $"LINGO is not detected. {LingoCardToString(card)}");
        }

        [MonitoredTest("HasLingo - Horizontal Lingo - Should return true")]
        public void _05_HasLingo_HorizontalLingo_ShouldReturnTrue()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCard(RandomGenerator.NextBool()) as ILingoCard;

            UncrossWholeCard(card);

            //cross out random row
            int lingoRowIndex = RandomGenerator.Next(0, 5);
            for (int j = 0; j < 5; j++)
            {
                card.CardNumbers[lingoRowIndex, j].CrossedOut = true;
            }

            //Act
            bool hasLingo = card.HasLingo;

            //Assert
            Assert.That(hasLingo, Is.True, $"LINGO is not detected. {LingoCardToString(card)}");
        }

       
        [MonitoredTest("HasLingo - LeftToRight Diagonal Lingo - Should return true")]
        public void _06_HasLingo_LeftToRightDiagonalLingo_ShouldReturnTrue()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCard(RandomGenerator.NextBool()) as ILingoCard;

            UncrossWholeCard(card);

            //cross out diagonal (left top to right bottom)
            for (int i = 0; i < 5; i++)
            {
                card.CardNumbers[i, i].CrossedOut = true;
            }

            //Act
            bool hasLingo = card.HasLingo;

            //Assert
            Assert.That(hasLingo, Is.True, $"LINGO is not detected. {LingoCardToString(card)}");
        }

        [MonitoredTest("HasLingo - RightToLeft Diagonal Lingo - Should return true")]
        public void _06_HasLingo_RightToLeftDiagonalLingo_ShouldReturnTrue()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCard(RandomGenerator.NextBool()) as ILingoCard;

            UncrossWholeCard(card);

            //cross out diagonal (left top to right bottom)
            for (int i = 4; i >=0 ; i--)
            {
                card.CardNumbers[i, 4-i].CrossedOut = true;
            }

            //Act
            bool hasLingo = card.HasLingo;

            //Assert
            Assert.That(hasLingo, Is.True, $"LINGO is not detected. {LingoCardToString(card)}");
        }

        [MonitoredTest("HasLingo - No Lingo Present - Should return false")]
        public void _07_HasLingo_NoLingoPresent_ShouldReturnFalse()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCard(RandomGenerator.NextBool()) as ILingoCard;

            UncrossWholeCard(card);

            //cross square of 4 numbers
            int iStart = RandomGenerator.Next(0, 2);
            int jStart = RandomGenerator.Next(0, 2);

            for (int i = iStart; i < iStart + 4; i++)
            {
                for (int j = jStart; j < jStart + 4; j++)
                {
                    card.CardNumbers[i, j].CrossedOut = true;
                }
            }

            //Act
            bool hasLingo = card.HasLingo;

            //Assert
            Assert.That(hasLingo, Is.False, $"A LINGO is detected while there isn't one. {LingoCardToString(card)}");
        }

        [MonitoredTest("Constructor - Should randomly cross out 8 numbers")]
        public void _08_Constructor_ShouldRandomlyCrossOut8Numbers()
        {
            AssertThatInterfaceIsNotChanged();

            int numberOfRuns = 500;

            for (int i = 0; i < numberOfRuns; i++)
            {
                //Act
                ILingoCard card = new LingoCard(RandomGenerator.NextBool()) as ILingoCard;

                //Assert
                IList<ICardNumber> generatedCardNumbers = GetAndAssertGeneratedCardNumbers(card);

                int numberOfCrossedOutNumbers = generatedCardNumbers.Count(cn => cn.CrossedOut);
                Assert.That(numberOfCrossedOutNumbers, Is.EqualTo(8), $"There should be 8 numbers crossed out. {LingoCardToString(card)}");

                Assert.That(card.HasLingo, Is.False, $"After construction the card should never have a LINGO. {LingoCardToString(card)}");
            }
        }

        [MonitoredTest("CrossOutNumber - Number is on card - Should cross out the number")]
        public void _09_CrossOutNumber_NumberIsOnCard_ShouldCrossOutTheNumber()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCard(RandomGenerator.NextBool()) as ILingoCard;

            UncrossWholeCard(card);
            int i = RandomGenerator.Next(0, 5);
            int j = RandomGenerator.Next(0, 5);
            int numberToCrossOut = card.CardNumbers[i, j].Value;

            //Act
            card.CrossOutNumber(numberToCrossOut);

            //Assert
            Assert.That(card.CardNumbers[i, j].CrossedOut, Is.True, $"The number {numberToCrossOut} was not crossed out. {LingoCardToString(card)}");
        }

        [MonitoredTest("CrossOutNumber - Number is not on card - Should do nothing")]
        public void _10_CrossOutNumber_NumberIsNotOnCard_ShouldDoNothing()
        {
            AssertThatInterfaceIsNotChanged();

            //Arrange
            ILingoCard card = new LingoCard(RandomGenerator.NextBool()) as ILingoCard;
            UncrossWholeCard(card);
            int numberNotOnCard = 100;

            //Act
            card.CrossOutNumber(numberNotOnCard);

            //Assert
            IList<ICardNumber> allCardNumbers = GetAndAssertGeneratedCardNumbers(card);
            Assert.That(allCardNumbers.All(cn => !cn.CrossedOut), Is.True, $"A number was crossed out. {LingoCardToString(card)}");
        }

        private void UncrossWholeCard(ILingoCard card)
        {
            Assert.That(card.CardNumbers, Is.Not.Null,
                "Cannot arrange the test correctly because the card does not contain numbers after construction. " +
                "Make sure the previous tests are green first.");

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    card.CardNumbers[i, j].CrossedOut = false;
                }
            }
        }

        private string LingoCardToString(ILingoCard card)
        {
            var builder = new StringBuilder();
            builder.AppendLine();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (card.CardNumbers[i, j].CrossedOut)
                    {
                        builder.Append($"({card.CardNumbers[i, j].Value})".PadRight(5));
                    }
                    else
                    {
                        builder.Append(card.CardNumbers[i, j].Value.ToString().PadRight(5));
                    }
                }
                builder.AppendLine();
            }
            builder.AppendLine();
            return builder.ToString();
        }

        private void TestNumberGeneration(bool useEvenNumbers)
        {
            IList<ICardNumber> previousCardNumbers = new List<ICardNumber>();

            int numberOfRuns = 25;
            int numberOfTimesTheSameValuesWhereGenerated = 0;

            for (int i = 0; i < numberOfRuns; i++)
            {
                //Act
                ILingoCard card = new LingoCard(useEvenNumbers) as ILingoCard;

                //Assert
                IList<ICardNumber> generatedCardNumbers = GetAndAssertGeneratedCardNumbers(card);
                if (useEvenNumbers)
                {
                    Assert.That(generatedCardNumbers.All(cn => cn.Value % 2 == 0), Is.True, $"Not all generated numbers are even. {LingoCardToString(card)}");
                }
                else
                {
                    Assert.That(generatedCardNumbers.All(cn => cn.Value % 2 != 0), Is.True, $"Not all generated numbers are odd. {LingoCardToString(card)}");
                }

                Assert.That(generatedCardNumbers.All(cn => cn.Value is >= 1 and <= 70), Is.True, $"Not all generated numbers are in the [1-70] range. {LingoCardToString(card)}");

                int numberOfSameValuesAsPreviouslyGeneratedValues = previousCardNumbers.Select(cn => cn.Value)
                    .Intersect(generatedCardNumbers.Select(cn => cn.Value)).Count();

                if (numberOfSameValuesAsPreviouslyGeneratedValues == generatedCardNumbers.Count)
                {
                    numberOfTimesTheSameValuesWhereGenerated++;
                }

                previousCardNumbers = generatedCardNumbers;
            }

            Assert.That(numberOfTimesTheSameValuesWhereGenerated, Is.LessThan(numberOfRuns / 4),
                "The generation of numbers does not seem random enough. Often exactly the same numbers are chosen for the card.");
        }

        private IList<ICardNumber> GetAndAssertGeneratedCardNumbers(ILingoCard card)
        {
            Assert.That(card.CardNumbers, Is.Not.Null, "The 'CardNumbers' property should not be null.");

            int numberOfRows = card.CardNumbers.GetLength(0);
            Assert.That(numberOfRows, Is.EqualTo(5), "There should be 5 rows of 'CardNumbers'.");
            int numberOfColumns = card.CardNumbers.GetLength(1);
            Assert.That(numberOfColumns, Is.EqualTo(5), "There should be 5 columns of 'CardNumbers'.");

            var generatedCardNumbers = new List<ICardNumber>();
            for (int i = 0; i < numberOfRows; i++)
            {
                for (int j = 0; j < numberOfColumns; j++)
                {
                    generatedCardNumbers.Add(card.CardNumbers[i, j]);
                }
            }

            var distinctCardNumbers = generatedCardNumbers.DistinctBy(cn => cn.Value).ToList();
            Assert.That(distinctCardNumbers.Count, Is.EqualTo(generatedCardNumbers.Count),
                $"The generated numbers must all have a different value. {LingoCardToString(card)}");

            return generatedCardNumbers;
        }

        private void AssertThatInterfaceIsNotChanged()
        {
            Assert.That(_iLingoCardHash, Is.EqualTo("31-82-82-AE-FB-BB-C8-68-E1-93-CA-A0-2F-35-A3-2C"),
                "The code of the ILingoCard interface has changed. This is not allowed. Undo your changes in 'ILingoCard.cs'");
        }
    }
}