using Lingo.Domain.Card.Contracts;
using Lingo.Domain.Contracts;
using Lingo.Domain.Puzzle.Contracts;

namespace Lingo.Domain
{
    internal class GameFactory : IGameFactory
    {
        public GameFactory(ILingoCardFactory cardFactory)
        {

        }

        public IGame CreateStandardGameForUsers(User user1, User user2, IList<IPuzzle> puzzles)
        {
            throw new NotImplementedException();
        }
    }
}