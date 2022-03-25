using AutoMapper;
using Lingo.Domain;

namespace Lingo.Api.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }
        public string NickName { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<User, UserModel>();
            }
        }
    }
}