using Lingo.AppLogic.Contracts;
using Lingo.Domain;
using Lingo.Domain.Contracts;
using Lingo.Domain.Pit.Contracts;

namespace Lingo.AppLogic
{
    /// <inheritdoc cref="IGameService"/>
    internal class GameService : IGameService
    {
        public GameService(IGameRepository gameRepository, IGameFactory gameFactory, IPuzzleService puzzleService)
        {

        }

        public void CreateGameForUsers(User user1, User user2, GameSettings settings)
        {
            throw new NotImplementedException();
        }

        public IList<IGame> GetGamesFor(Guid userId)
        {
            throw new NotImplementedException();
        }

        public IGame GetById(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public SubmissionResult SubmitAnswer(Guid gameId, Guid playerId, string answer)
        {
            throw new NotImplementedException();
        }

        public IBall GrabBallFromBallPit(Guid gameId, Guid playerId)
        {
            throw new NotImplementedException();
        }
    }
}
