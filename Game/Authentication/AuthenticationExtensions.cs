using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RestAdventure.Core.Players;
using RestAdventure.Core.Serialization;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Game.Authentication;

static class AuthenticationExtensions
{
    public static UserId? GetUserId(this ControllerContext context)
    {
        string? guidStr = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (guidStr == null)
        {
            return null;
        }

        return Guid.TryParse(guidStr, out Guid playerGuid) ? new UserId(playerGuid) : null;
    }

    public static UserId RequireUserId(this ControllerContext context) => GetUserId(context) ?? throw new InvalidOperationException("Could not determine current user");

    public static Player? GetPlayer(this ControllerContext context, Core.Game state)
    {
        UserId? userId = GetUserId(context);
        return userId == null ? null : state.Players.GetPlayer(userId);
    }

    public static PlayerSnapshot? GetPlayer(this ControllerContext context, GameSnapshot snapshot)
    {
        UserId? userId = GetUserId(context);
        return userId == null ? null : snapshot.Players.GetValueOrDefault(userId);
    }

    public static Player RequirePlayer(this ControllerContext context, Core.Game state) =>
        GetPlayer(context, state) ?? throw new InvalidOperationException("Could not determine current player");

    public static PlayerSnapshot RequirePlayer(this ControllerContext context, GameSnapshot snapshot) =>
        GetPlayer(context, snapshot) ?? throw new InvalidOperationException("Could not determine current player");
}
