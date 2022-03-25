namespace Lingo.Domain.Card.Contracts
{
    /// <summary>
    /// A card containing 25 random numbers.
    /// All numbers are either even or odd.
    /// All numbers must be in the 1-70 range.
    /// 8 numbers on the card are randomly crossed out (without causing a LINGO) when a card is created.
    /// </summary>
    public interface ILingoCard
    {
        /// <summary>
        /// The numbers on the card (5 rows, 5 columns)
        /// </summary>
        ICardNumber[,] CardNumbers { get; }

        /// <summary>
        /// Indicates if the card has 5 sequential crossed out numbers (=LINGO). This can be in a horizontal, vertical or diagonal direction
        /// </summary>
        bool HasLingo { get; }

        /// <summary>
        /// Crosses out a <paramref name="number"/> on the card
        /// </summary>
        /// <param name="number">The number to be crossed out</param>
        void CrossOutNumber(int number);
    }
}