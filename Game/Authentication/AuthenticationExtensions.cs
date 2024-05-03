using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace RestAdventure.Game.Authentication;

static class AuthenticationExtensions
{
    public static Guid? GetPlayerId(this ControllerContext context)
    {
        string? guidStr = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (guidStr == null)
        {
            return null;
        }

        return Guid.TryParse(guidStr, out Guid playerId) ? playerId : null;
    }

    public static Guid RequirePlayerId(this ControllerContext context) => GetPlayerId(context) ?? throw new InvalidOperationException("Could not determine current player");
}
