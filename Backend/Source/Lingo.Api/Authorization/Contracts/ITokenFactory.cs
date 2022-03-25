using Lingo.Domain;

namespace Lingo.Api.Authorization.Contracts
{
    //DO NOT TOUCH THIS FILE!!

    public interface ITokenFactory
    {
        string CreateToken(User user, IList<string> roleNames);
    }
}