using Lingo.Domain;

namespace Lingo.TestTools.Builders
{
    public class UserBuilder
    {
        private readonly User _user;

        public UserBuilder()
        {
            _user = new User
            {
                Id = Guid.NewGuid(),
                Email = Guid.NewGuid().ToString(),
                NickName = Guid.NewGuid().ToString(),
                UserName = Guid.NewGuid().ToString(),
                PasswordHash = Guid.NewGuid().ToString()
            };
        }

        public User Build()
        {
            return _user;
        }
    }
}