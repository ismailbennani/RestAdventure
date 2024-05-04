using Microsoft.AspNetCore.Authorization;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Controllers;

namespace RestAdventure.Game.Apis.GameApi;

/// <summary>
///     Base class for Game API controllers
/// </summary>
[Authorize(AuthenticationSchemes = GameApiAuthenticationOptions.AuthenticationScheme)]
[GameApi]
public abstract class GameApiController : ApiController
{
}
