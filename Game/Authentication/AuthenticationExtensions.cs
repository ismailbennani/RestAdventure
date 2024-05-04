using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RestAdventure.Core.Players;

namespace RestAdventure.Game.Authentication;

static class AuthenticationExtensions
{
    public static PlayerId? GetPlayerId(this ControllerContext context)
    {
        string? guidStr = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (guidStr == null)
        {
            return null;
        }

        return Guid.TryParse(guidStr, out Guid playerGuid) ? new PlayerId(playerGuid) : null;
    }

    public static PlayerId RequirePlayerId(this ControllerContext context) => GetPlayerId(context) ?? throw new InvalidOperationException("Could not determine current player");
}
