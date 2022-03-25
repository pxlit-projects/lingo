using Lingo.Domain;
using Lingo.Domain.Puzzle;
using Lingo.Domain.Puzzle.Contracts;
using Moq;

namespace Lingo.TestTools.Builders;

public class WordPuzzleMockBuilder : MockBuilder<IWordPuzzle>
{
    public WordPuzzleMockBuilder()
    {
        Mock.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        Mock.SetupGet(p => p.IsFinished).Returns(false);
        Mock.SetupGet(p => p.Score).Returns(0);
        Mock.SetupGet(p => p.Guesses).Returns(new List<WordGuess>());
        Mock.SetupGet(p => p.RevealedLetters).Returns(".....".ToCharArray());
        Mock.Setup(p => p.SubmitAnswer(It.IsAny<string>())).Returns(SubmissionResult.CreateKeepTurnResult());
    }

    public WordPuzzleMockBuilder WithIsFinished(bool isFinished)
    {
        Mock.SetupGet(p => p.IsFinished).Returns(isFinished);
        return this;
    }

    public WordPuzzleMockBuilder WithSubmissionResult(SubmissionResult result, bool causesThePuzzleToFinish = false)
    {
        Mock.Setup(p => p.SubmitAnswer(It.IsAny<string>())).Returns(() =>
        {
            Mock.SetupGet(p => p.IsFinished).Returns(causesThePuzzleToFinish);
            return result;
        });
       
        return this;
    }
}