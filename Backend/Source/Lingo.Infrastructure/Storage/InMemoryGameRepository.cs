
using Lingo.AppLogic.Contracts;
using Lingo.Common;
using Lingo.Domain.Contracts;

namespace Lingo.Infrastructure.Storage
{
    /// <summary>
    /// Stores all the games in an in-memory dictionary on the server.
    /// Games are removed from the dictionary automatically after 5 hours.
    /// </summary>
    /// <remarks>There should be no need to alter any code in this class.</remarks>
    internal class InMemoryGameRepository : IGameRepository
    {
        private readonly ExpiringDictionary<Guid, IGame> _gameDictionary;

        public InMemoryGameRepository()
        {
            _gameDictionary = new ExpiringDictionary<Guid, IGame>(TimeSpan.FromHours(5));
        }

        public void Add(IGame newGame)
        {
            _gameDictionary.AddOrReplace(newGame.Id, newGame);
        }

        public IGame GetById(Guid id)
        {
            if (_gameDictionary.TryGetValue(id, out IGame game))
            {
                return game;
            }
            throw new DataNotFoundException();
        }

        public IList<IGame> GetGamesOfUser(Guid userId)
        {
            IList<IGame> matchingGames = _gameDictionary.Values.Where(game => game.Player1.Id == userId || game.Player2.Id == userId).ToList();
            return matchingGames;
        }
    }
}