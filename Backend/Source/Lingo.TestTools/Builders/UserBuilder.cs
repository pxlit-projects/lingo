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

        public UserBuilder AsCloneOf(User user)
        {
            _user.Id = user.Id;
            _user.Email = user.Email;
            _user.NickName = user.NickName;
            _user.UserName = user.UserName;
            _user.PasswordHash = user.PasswordHash;
            return this;
        }
        public User Build()
        {
            return _user;
        }
    }
}