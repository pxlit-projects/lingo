namespace Lingo.Domain.Card.Contracts
{
    /// <summary>
    /// A number on a <see cref="ILingoCard"/>
    /// </summary>
    public interface ICardNumber
    {
        bool CrossedOut { get; set; }
        int Value { get; }
    }
}