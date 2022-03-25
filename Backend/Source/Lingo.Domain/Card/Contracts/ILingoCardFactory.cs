namespace Lingo.Domain.Card.Contracts;

/// <summary>
/// Factory that can create instances of classes that implement <see cref="ILingoCard"/>
/// </summary>
public interface ILingoCardFactory
{
    /// <summary>
    /// Creates a new instance of a class that implements <see cref="ILingoCard"/>.
    /// </summary>
    /// <param name="useEvenNumbers">Indicated if the card should have all even or all odd numbers</param>
    /// <returns>The created card</returns>
    ILingoCard CreateNew(bool useEvenNumbers);
}