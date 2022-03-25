using Lingo.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Lingo.Api.Controllers
{
    [Route("")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [Route("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return RedirectPermanent("~/swagger");
        }

        /// <summary>
        /// Shows that the api is up and running.
        /// </summary>
        [HttpGet("ping")]
        [ProducesResponseType(typeof(PingResultModel), StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            var model = new PingResultModel
            {
                IsAlive = true,
                Greeting = $"Hello on this fine {DateTime.Now.DayOfWeek}"
            };
            return Ok(model);
        }
    }
}