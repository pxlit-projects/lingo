using AutoMapper;
using Lingo.Domain.Contracts;

namespace Lingo.Api.Models
{
    public class PlayerModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<IPlayer, PlayerModel>();
            }
        }
    }
}