using AutoMapper;
using Lingo.Api.Models;
using Lingo.AppLogic.Contracts;
using Lingo.Domain;
using Lingo.Domain.Contracts;
using Lingo.Domain.Pit.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Lingo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ApiControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;

        public GameController(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get the games the current user can play (because they were created by an administrator)
        /// </summary>
        [HttpGet("my-scheduled-games")]
        [ProducesResponseType(typeof(IList<GameListItemModel>), StatusCodes.Status200OK)]
        public IActionResult GetMyGames()
        {
            IList<IGame> games = _gameService.GetGamesFor(UserId);
            IList<GameListItemModel> model = games.Select(game => _mapper.Map<GameListItemModel>(game)).ToList();
            return Ok(model);
        }

        /// <summary>
        /// Gets information about a game
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetGame(Guid id)
        {
            IGame game = _gameService.GetById(id);
            if(game.Player1.Id != UserId && game.Player2.Id != UserId)
            {
                return BadRequest(new ErrorModel($"De gebruiker met id '{UserId}' is geen speler van het spel."));
            }
            return Ok(game);
        }

        /// <summary>
        /// Submits an answer for the current puzzle of the game.
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        /// <param name="model">Contains the submitted answer</param>
        [HttpPost("{id}/submit-answer")]
        [ProducesResponseType(typeof(SubmissionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult SubmitAnswer(Guid id, [FromBody] AnswerModel model)
        {
             SubmissionResult result = _gameService.SubmitAnswer(id, UserId, model.Answer);
             return Ok(result);
        }

        /// <summary>
        /// Grabs a ball from the ball pit of the user (player) in the game.  
        /// </summary>
        /// <param name="id">Id (guid) of the game</param>
        [HttpPost("{id}/grab-ball")]
        [ProducesResponseType(typeof(IBall), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public IActionResult GrabBall(Guid id)
        {
            IBall ball = _gameService.GrabBallFromBallPit(id, UserId);
            return Ok(ball);
        }
    }
}
