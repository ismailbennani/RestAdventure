using RestAdventure.Core.Players;
using RestAdventure.Game.Registration;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Authentication;

class PlayerAuthenticationService
{
    readonly DomainAccessor _domainAccessor;
    readonly Dictionary<Guid, PlayerSession> _sessions = new();

    public PlayerAuthenticationService(DomainAccessor domainAccessor)
    {
        _domainAccessor = domainAccessor;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(Guid apiKey)
    {
        if (!_sessions.TryGetValue(apiKey, out PlayerSession? session))
        {
            session = await LoadSessionFromPersistence(apiKey);

            if (session == null)
            {
                return AuthenticationResult.Failure();
            }

            _sessions[apiKey] = session;
        }

        session.LastActivityDate = DateTime.Now;

        return AuthenticationResult.Success(session);
    }

    async Task<PlayerSession?> LoadSessionFromPersistence(Guid apiKey)
    {
        await using Session? session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope? transaction = await session.OpenTransactionAsync();

        PlayerDbo? player = await session.Query.All<PlayerRegistrationDbo>().Where(r => r.ApiKey == apiKey).Select(r => r.Player).SingleOrDefaultAsync();
        if (player == null)
        {
            return null;
        }

        return new PlayerSession(player.Id, player.Name);
    }
}
