using Lingo.Domain;

namespace Lingo.Api.Models
{
    public class GameCreationModel
    {
        public Guid User1Id { get; set; }

        public Guid User2Id { get; set; }

        public GameSettings Settings { get; set; }
    }
}