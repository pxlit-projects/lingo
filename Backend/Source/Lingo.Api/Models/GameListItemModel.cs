using AutoMapper;
using Lingo.Domain.Contracts;

namespace Lingo.Api.Models
{
    public class GameListItemModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
      

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<IGame, GameListItemModel>()
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => $"{src.Player1.Name} vs. {src.Player2.Name}"));
            }
        }
    }
}