using Lingo.Domain.Contracts;

namespace Lingo.Domain;

public class RankingStrategy : IRankingStrategy
{
    public void AdjustRanking(IList<User> userSlice, User player1User, int player1GameScore, User player2User, int player2GameScore)
    {
        throw new NotImplementedException();
    }
}