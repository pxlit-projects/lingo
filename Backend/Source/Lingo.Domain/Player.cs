using Lingo.Domain.Card.Contracts;
using Lingo.Domain.Contracts;
using Lingo.Domain.Pit.Contracts;

namespace Lingo.Domain
{
    /// <inheritdoc cref="IPlayer"/>
    internal class Player
    {
        public Player(Guid id, string name, IBallPit ballPit, ILingoCardFactory cardFactory, bool useEvenNumbers)
        {
            
        }
    }
}