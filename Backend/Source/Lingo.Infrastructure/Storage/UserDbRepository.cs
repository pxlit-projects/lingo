using Lingo.AppLogic.Contracts;
using Lingo.Common;
using Lingo.Domain;

namespace Lingo.Infrastructure.Storage
{
    /// <summary>
    /// Repository to retrieve and manipulate users in the database
    /// </summary>
    internal class UserDbRepository : IUserRepository
    {
        private LingoDbContext _context;

        public UserDbRepository(LingoDbContext context)
        {
            _context = context;
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
            var user =  _context.Users.FirstOrDefault(user => user.Id == id);
            if(user == null)
            {
                throw new DataNotFoundException();
            }
            return user;
        }
    }
}