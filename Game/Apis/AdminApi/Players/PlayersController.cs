using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Game.Apis.AdminApi.Players.Dtos;
using RestAdventure.Game.Apis.AdminApi.Players.Requests;
using RestAdventure.Game.Registration;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.AdminApi.Players;

[Route("admin/players")]
[ApiController]
[AdminApi]
[OpenApiTag("Players")]
public class PlayersController : ControllerBase
{
    readonly DomainAccessor _domainAccessor;

    public PlayersController(DomainAccessor domainAccessor)
    {
        _domainAccessor = domainAccessor;
    }

    /// <summary>
    ///     Register player
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PlayerRegistrationDto>> RegisterPlayer(RegisterPlayerRequestDto request)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        PlayerRegistrationDbo? existingRegistration = await GetExistingPlayerRegistration(session, request.PlayerId);
        if (existingRegistration != null)
        {
            existingRegistration.Player.Name = request.PlayerName;
            return existingRegistration.ToDto();
        }

        PlayerRegistrationDbo registration;
        using (session.Activate())
        {
            registration = new PlayerRegistrationDbo(request.PlayerId, request.PlayerName);
        }

        transaction.Complete();

        return registration.ToDto();
    }

    /// <summary>
    ///     Get registration
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PlayerRegistrationDto>> GetApiKey(Guid playerId)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        PlayerRegistrationDbo? existingRegistration = await GetExistingPlayerRegistration(session, playerId);
        if (existingRegistration == null)
        {
            return Problem($"Could not find registration of player {playerId}", statusCode: StatusCodes.Status400BadRequest);
        }

        return existingRegistration.ToDto();
    }

    /// <summary>
    ///     Refresh registration
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<PlayerRegistrationDto>> RefreshApiKey(Guid playerId)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        PlayerRegistrationDbo? existingRegistration = await GetExistingPlayerRegistration(session, playerId);
        if (existingRegistration == null)
        {
            return Problem($"Could not refresh registration of player {playerId}", statusCode: StatusCodes.Status400BadRequest);
        }

        PlayerRegistrationDbo newRegistration;
        using (session.Activate())
        {
            newRegistration = new PlayerRegistrationDbo(existingRegistration.Player.Id, existingRegistration.Player.Name);
            existingRegistration.Remove();
        }

        transaction.Complete();

        return newRegistration.ToDto();
    }

    static async Task<PlayerRegistrationDbo?> GetExistingPlayerRegistration(Session session, Guid playerId) =>
        await session.Query.All<PlayerRegistrationDbo>().FirstOrDefaultAsync(r => r.Player.Id == playerId);
}
