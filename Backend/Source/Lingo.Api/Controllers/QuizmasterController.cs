using AutoMapper;
using Lingo.Api.Models;
using Lingo.AppLogic.Contracts;
using Lingo.Common;
using Lingo.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lingo.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = AppConstants.QuizmastersOnlyPolicyName)]
    [ApiController]
    public class QuizmasterController : ApiControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;

        public QuizmasterController(IUserRepository userRepository,IGameService gameService, IMapper mapper)
        {
            _userRepository = userRepository;
            _gameService = gameService;
            _mapper = mapper;
        }

        /// <summary>
        /// Finds users that can be scheduled for a game.
        /// </summary>
        /// <param name="filter">
        /// Optional filter. Searches for a match on email or nickname
        /// </param>
        [HttpGet("users")]
        [ProducesResponseType(typeof(IList<UserModel>), StatusCodes.Status200OK)]
        public IActionResult FindUsers(string filter)
        {
            IList<User> matchingUsers = _userRepository.FindUsers(filter);
            IList<UserModel> model = matchingUsers.Select(user => _mapper.Map<UserModel>(user)).ToList();
            return Ok(model);
        }

        /// <summary>
        /// Creates a game for 2 human players.
        /// </summary>
        [HttpPost("create-game")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult CreateGame(GameCreationModel model)
        {
            User user1 = _userRepository.GetById(model.User1Id);
            User user2 = _userRepository.GetById(model.User2Id);
            _gameService.CreateGameForUsers(user1, user2, model.Settings);
            return Ok();
        }
    }
}
