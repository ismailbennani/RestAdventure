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

    public async Task<PlayerRegistrationDto?> RegisterPlayer(Guid playerId, string playerName)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        PlayerRegistrationDbo? existingRegistration = await GetExistingPlayerRegistration(session, playerId);
        if (existingRegistration != null)
        {
            existingRegistration.Player.Name = playerName;
            return existingRegistration.ToDto();
        }

        PlayerRegistrationDbo registration;
        using (session.Activate())
        {
            registration = new PlayerRegistrationDbo(playerId, playerName);
        }

        transaction.Complete();

        return registration.ToDto();
    }

    public async Task<PlayerRegistrationDto?> GetRegistration(Guid playerId)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        PlayerRegistrationDbo? existingRegistration = await GetExistingPlayerRegistration(session, playerId);

        return existingRegistration?.ToDto();
    }

    public async Task<PlayerRegistrationDto?> RefreshApiKey(Guid apiKey)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        PlayerRegistrationDbo? existingRegistration = await GetExistingPlayerRegistration(session, apiKey);
        if (existingRegistration == null)
        {
            return null;
        }

        Guid playerId = existingRegistration.Player.Id;
        string playerName = existingRegistration.Player.Name;

        PlayerRegistrationDbo newRegistration;
        using (session.Activate())
        {
            newRegistration = new PlayerRegistrationDbo(playerId, playerName);
            existingRegistration.Remove();
        }

        transaction.Complete();

        return newRegistration.ToDto();
    }

    static async Task<PlayerRegistrationDbo?> GetExistingPlayerRegistration(Session session, Guid playerId) =>
        await session.Query.All<PlayerRegistrationDbo>().FirstOrDefaultAsync(r => r.Player.Id == playerId);
}
