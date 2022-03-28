using System.Collections.Generic;
using AutoMapper;
using Guts.Client.Core;
using Lingo.Api.Controllers;
using Lingo.Api.Models;
using Lingo.AppLogic.Contracts;
using Lingo.Domain;
using Lingo.TestTools;
using Lingo.TestTools.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Lingo.Api.Tests;

[ProjectComponentTestFixture("1TINProject", "Lingo", "QuizmasterCtlr", @"Lingo.Api\Controllers\QuizmasterController.cs")]
public class QuizMasterControllerTests : TestBase
{
    private QuizmasterController _controller;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IGameService> _gameServiceMock;
    private Mock<IMapper> _mapperMock;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        _gameServiceMock = new Mock<IGameService>();

        _mapperMock = new Mock<IMapper>();

        _controller = new QuizmasterController(_userRepositoryMock.Object, _gameServiceMock.Object, _mapperMock.Object);
    }

    [MonitoredTest("FindUsers - Should retrieve all matching users")]
    public void FindUsers_ShouldRetrieveAllMatchingUsers()
    {
        //Arrange
        string filter = RandomGenerator.NextString();
        IList<User> matchingUsers = new List<User>
        {
            new UserBuilder().Build()
        };

        _userRepositoryMock.Setup(repo => repo.FindUsers(It.IsAny<string>())).Returns(matchingUsers);

        var dummyUserModel = new UserModel();
        _mapperMock.Setup(mapper => mapper.Map<UserModel>(It.IsAny<object>())).Returns(dummyUserModel);

        //Act
        var result = _controller.FindUsers(filter) as OkObjectResult;

        //Assert
        _userRepositoryMock.Verify(repo => repo.FindUsers(filter), Times.Once,
            "The 'FindUsers' method of the user repository is not called correctly");
        Assert.That(result, Is.Not.Null, "An instance of 'OkObjectResult' should be returned.");

        _mapperMock.Verify(mapper => mapper.Map<UserModel>(It.IsIn<User>(matchingUsers)), Times.Exactly(matchingUsers.Count),
            "The users returned by the repository should be mapped to instances of 'UserModel'.");

        var userModels = result.Value as IList<UserModel>;
        Assert.That(userModels, Is.Not.Null, "The result should contain a list of user models.");
        Assert.That(userModels.Count, Is.EqualTo(matchingUsers.Count),
            "The number of models returned should be the same as the number of users returned by the repository.");
    }

    [MonitoredTest("CreateGame - Should retrieve users and use service")]
    public void CreateGame_ShouldRetrieveUsersAndUseService()
    {
        //Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var model = new GameCreationModel
        {
            Settings = new GameSettings(),
            User1Id = user1.Id,
            User2Id = user2.Id
        };

        _userRepositoryMock.Setup(repo => repo.GetById(user1.Id)).Returns(user1);
        _userRepositoryMock.Setup(repo => repo.GetById(user2.Id)).Returns(user2);

        //Act
        var result = _controller.CreateGame(model) as OkResult;

        //Assert
        _userRepositoryMock.Verify(repo => repo.GetById(user1.Id), Times.Once,
            "The 'GetById' method of the user repository is not called correctly for user 1");
        _userRepositoryMock.Verify(repo => repo.GetById(user2.Id), Times.Once,
            "The 'GetById' method of the user repository is not called correctly for user 2");

        _gameServiceMock.Verify(service => service.CreateGameForUsers(user1, user2, model.Settings), Times.Once,
            "The 'CreateGameForUsers' method of the game service is not called correctly.");

        Assert.That(result, Is.Not.Null, "An instance of 'OkResult' should be returned.");
    }
}