using System;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Guts.Client.Core;
using Lingo.Api.Controllers;
using Lingo.Api.Models;
using Lingo.AppLogic.Contracts;
using Lingo.Domain;
using Lingo.Domain.Contracts;
using Lingo.Domain.Pit.Contracts;
using Lingo.TestTools;
using Lingo.TestTools.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Lingo.Api.Tests
{
    [ProjectComponentTestFixture("1TINProject", "Lingo", "GameCtlr", @"Lingo.Api\Controllers\GameController.cs")]
    public class GameControllerTests : TestBase
    {
        private GameController _controller;
        private Mock<IGameService> _gameServiceMock;
        private Mock<IMapper> _mapperMock;
        private User _loggedInUser;
        private GameMockBuilder _gameMockBuilder;
        private IGame _game;

        [SetUp]
        public void Setup()
        {
            _loggedInUser = new UserBuilder().Build();

            _gameServiceMock = new Mock<IGameService>();
            _gameMockBuilder = new GameMockBuilder();
            _game = _gameMockBuilder.Object;
            _gameMockBuilder.Player1MockBuilder.WithId(_loggedInUser.Id);
            _gameServiceMock.Setup(service => service.GetById(It.IsAny<Guid>())).Returns(_game);
            
            _mapperMock = new Mock<IMapper>();

            _controller = new GameController(_gameServiceMock.Object, _mapperMock.Object);

            var userClaimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, _loggedInUser.Id.ToString())
                })
            );
            var context = new ControllerContext { HttpContext = new DefaultHttpContext() };
            context.HttpContext.User = userClaimsPrincipal;
            _controller.ControllerContext = context;
        }

        [MonitoredTest("GetMyGames - Should retrieve the games of the logged in user from the game service")]
        public void GetMyGames_ShouldRetrieveTheGamesOfTheLoggedInUserFromTheGameService()
        {
            //Arrange
            var myGames = new List<IGame> { _game };
            _gameServiceMock.Setup(service => service.GetGamesFor(It.IsAny<Guid>())).Returns(myGames);

            var dummyListItemModel = new GameListItemModel();
            _mapperMock.Setup(mapper => mapper.Map<GameListItemModel>(It.IsAny<object>())).Returns(dummyListItemModel);

            //Act
            var result = _controller.GetMyGames() as OkObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.GetGamesFor(_loggedInUser.Id), Times.Once,
                "The 'GetGamesFor' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

            _mapperMock.Verify(mapper => mapper.Map<GameListItemModel>(_game), Times.Once,
                "The games returned by the service should be mapped to instances of 'GameListItemModel'.");

            var gameListItems = result.Value as IList<GameListItemModel>;
            Assert.That(gameListItems, Is.Not.Null, "The result should contain a list of game list item models.");
            Assert.That(gameListItems.Count, Is.EqualTo(myGames.Count),
                "The number of models returned should be the same as the number of games returned by the service.");
        }

        [MonitoredTest("GetGame - Should retrieve a game from the game service")]
        public void GetGame_ShouldRetrieveAGameFromTheGameService()
        {
            //Act
            var result = _controller.GetGame(_game.Id) as OkObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
            Assert.That(result.Value, Is.SameAs(_game), "The result should contain the game returned by the game service");
        }

        [MonitoredTest("GetGame - Logged in user is not a player of the game - Should return bad request")]
        public void GetGame_LoggedInUserIsNotAPlayerOfTheGame_ShouldReturnBadRequest()
        {
            //Arrange
            _gameMockBuilder.Player1MockBuilder.WithId(Guid.NewGuid());
            _gameMockBuilder.Player2MockBuilder.WithId(Guid.NewGuid());

            //Act
            var result = _controller.GetGame(_game.Id) as BadRequestObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.GetById(_game.Id), Times.Once,
                "The 'GetById' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'BadRequestObjectResult' should be returned.");
            var errorModel = result.Value as ErrorModel;
            Assert.That(errorModel, Is.Not.Null, "The bad request result should contain an instance of ErrorModel.");
        }

        [MonitoredTest("SubmitAnswer - Should return the submission result of the game service")]
        public void SubmitAnswer_ShouldReturnTheSubmissionResultOfTheGameService()
        {
            //Arrange
            var model = new AnswerModel
            {
                Answer = RandomGenerator.NextWord()
            };

            SubmissionResult submissionResult = SubmissionResult.CreateKeepTurnResult();
            _gameServiceMock
                .Setup(service => service.SubmitAnswer(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(submissionResult);

            //Act
            var result = _controller.SubmitAnswer(_game.Id, model) as OkObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.SubmitAnswer(_game.Id, _loggedInUser.Id, model.Answer), Times.Once,
                "The 'SubmitAnswer' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
            Assert.That(result.Value, Is.SameAs(submissionResult), "The result should contain the result returned by the game service");
        }

        [MonitoredTest("GrabBall - Should return the ball returned by the game service")]
        public void GrabBall_ShouldReturnTheBallReturnedByTheGameService()
        {
            //Arrange
            IBall ball = new BallMockBuilder().Object;

            _gameServiceMock
                .Setup(service => service.GrabBallFromBallPit(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(ball);

            //Act
            var result = _controller.GrabBall(_game.Id) as OkObjectResult;

            //Assert
            _gameServiceMock.Verify(service => service.GrabBallFromBallPit(_game.Id, _loggedInUser.Id), Times.Once,
                "The 'GrabBallFromBallPit' method of the game service is not called correctly");
            Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");
            Assert.That(result.Value, Is.SameAs(ball), "The result should contain the ball returned by the game service");
        }
    }
}