using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Lingo.Api.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Returns the id (Guid) of the authenticated user / player.
        /// If no user is authenticated an empty guid is returned.
        /// </summary>
        protected Guid UserId
        {
            get
            {
                if (User == null) return Guid.Empty;
                string idClaimValue = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                return Guid.TryParse(idClaimValue, out Guid userId) ? userId : Guid.Empty;
            }
        }
    }
}