using RestAdventure.Core.Players;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Registration;

public class PlayerRegistrationService
{
    readonly DomainAccessor _domainAccessor;

    public PlayerRegistrationService(DomainAccessor domainAccessor)
    {
        _domainAccessor = domainAccessor;
    }

    public async Task<PlayerRegistrationDto?> RegisterPlayer(Guid playerId)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        PlayerRegistrationDbo? existingRegistration = await GetExistingPlayerRegistration(session, playerId);
        if (existingRegistration != null)
        {
            return existingRegistration.ToDto();
        }

        PlayerDbo? player = await GetPlayer(session, playerId);
        if (player == null)
        {
            return null;
        }

        PlayerRegistrationDbo registration = new(player);

        transaction.Complete();

        return registration.ToDto();
    }

    static async Task<PlayerRegistrationDbo?> GetExistingPlayerRegistration(Session session, Guid playerId) =>
        await session.Query.All<PlayerRegistrationDbo>().FirstOrDefaultAsync(r => r.Player.Id == playerId);

    static async Task<PlayerDbo?> GetPlayer(Session session, Guid playerId) => await session.Query.All<PlayerDbo>().SingleOrDefaultAsync(p => p.Id == playerId);
}
