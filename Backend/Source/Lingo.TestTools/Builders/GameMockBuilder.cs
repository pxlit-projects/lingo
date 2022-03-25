using Lingo.Domain;
using Lingo.Domain.Contracts;
using Moq;

namespace Lingo.TestTools.Builders
{
    public class GameMockBuilder : MockBuilder<IGame>
    {
        public PlayerMockBuilder Player1MockBuilder { get; }
        public PlayerMockBuilder Player2MockBuilder { get; }
        public WordPuzzleMockBuilder CurrentPuzzleMockBuilder { get; }

        public GameMockBuilder()
        {
            Mock.SetupGet(g => g.Id).Returns(Guid.NewGuid());

            Player1MockBuilder = new PlayerMockBuilder();
            Player2MockBuilder = new PlayerMockBuilder();
           
            IPlayer player1 = Player1MockBuilder.Mock.Object;
            IPlayer player2 = Player2MockBuilder.Mock.Object;
            WithPlayers(player1, player2);

            Mock.SetupGet(g => g.Finished).Returns(false);

            CurrentPuzzleMockBuilder = new WordPuzzleMockBuilder();
            Mock.SetupGet(g => g.CurrentPuzzle).Returns(CurrentPuzzleMockBuilder.Object);

            Mock.Setup(g => g.SubmitAnswer(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(SubmissionResult.CreateKeepTurnResult());
        }

        public GameMockBuilder WithPlayers(IPlayer player1, IPlayer player2)
        {
            Mock.SetupGet(g => g.Player1).Returns(player1);
            Mock.SetupGet(g => g.Player2).Returns(player2);
            Mock.SetupGet(g => g.PlayerToPlayId).Returns(player1.Id);
            return this;
        }
    }
}