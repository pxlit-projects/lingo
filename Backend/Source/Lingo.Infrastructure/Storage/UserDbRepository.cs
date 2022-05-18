using Lingo.AppLogic.Contracts;
using Lingo.Common;
using Lingo.Domain;
using Lingo.Domain.Contracts;

namespace Lingo.Infrastructure.Storage
{
    /// <summary>
    /// Repository to retrieve and manipulate users in the database
    /// </summary>
    internal class UserDbRepository : IUserRepository
    {
        private LingoDbContext _context;
        private IRankingStrategy _rankingStrategy;

        public UserDbRepository(LingoDbContext context, IRankingStrategy rankingStrategy)
        {
            _context = context;
            _rankingStrategy = rankingStrategy;
        }

        public IList<User> FindUsers(string filter)
        {
            filter = filter?.ToLower();
            var query = from user in _context.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId into userRoles
                        from userRole in userRoles.DefaultIfEmpty()
                        join role in _context.Roles on userRole.RoleId equals role.Id into roles
                        from role in roles.DefaultIfEmpty()
                        where (role == null || role.Name! != "Quizmaster")
                        && (string.IsNullOrEmpty(filter)
                            || user.UserName.ToLower().Contains(filter)
                            || user.Email.ToLower().Contains(filter)
                            || user.NickName.ToLower().Contains(filter))
                        select user;
            return query.ToList();
        }

        public User GetById(Guid id)
        {
            var user = _context.Users.FirstOrDefault(user => user.Id == id);
            if (user == null)
            {
                throw new DataNotFoundException();
            }
            return user;
        }

        public void AdjustRankingAfterGame(IGame game)
        {
            //DO NOT CHANGE THIS METHOD!! Implement the necessary logic in the RankingStrategy class.
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                User user1 = _context.Users.Find(game.Player1.Id);
                User user2 = _context.Users.Find(game.Player2.Id);

                int fromRank = Math.Min(user1.Rank, user2.Rank) - 1;
                int toRank = Math.Max(user1.Rank, user2.Rank) + 1;

                IList<User> userSlice =
                    _context.Users.Where(u => u.Rank >= fromRank && u.Rank <= toRank).OrderByDescending(u => u.Rank)
                        .ToList();

                _rankingStrategy.AdjustRanking(userSlice, user1, game.Player1.Score, user2, game.Player2.Score);

                _context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }
}