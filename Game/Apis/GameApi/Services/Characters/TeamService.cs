using Microsoft.Extensions.Options;
using RestAdventure.Core.Characters;
using RestAdventure.Game.Apis.GameApi.Controllers.Characters.Requests;
using RestAdventure.Game.Controllers;
using RestAdventure.Game.Settings;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.GameApi.Services.Characters;

public class TeamService
{
    readonly IOptions<GameSettings> _gameSettings;

    public TeamService(IOptions<GameSettings> gameSettings)
    {
        _gameSettings = gameSettings;
    }

    public async Task<OperationResult<TeamDbo>> GetTeamAsync(Guid playerId) => OperationResult.Ok(await GetOrCreateTeamAsync(playerId));

    public async Task<OperationResult<CharacterDbo>> CreateCharacterAsync(Guid playerId, CreateCharacterRequestDto request)
    {
        TeamDbo team = await GetOrCreateTeamAsync(playerId);

        long nCharacters = await team.Characters.CountAsync();
        int maxTeamSize = _gameSettings.Value.MaxTeamSize;
        if (nCharacters >= maxTeamSize)
        {
            return OperationResult.BadRequest<CharacterDbo>($"reached max team size ({maxTeamSize})");
        }

        CharacterDbo character = new(team, request.Name, request.Class);

        return OperationResult.Ok(character);
    }

    public async Task<OperationResult<CharacterDbo>> GetCharacterAsync(Guid playerId, Guid characterId)
    {
        TeamDbo? team = await GetTeamInternalAsync(playerId);
        if (team == null)
        {
            return OperationResult.NotFound<CharacterDbo>();
        }

        CharacterDbo? character = await team.Characters.SingleOrDefaultAsync(c => c.Id == characterId);
        if (character == null)
        {
            return OperationResult.NotFound<CharacterDbo>();
        }

        return OperationResult.Ok(character);
    }

    public async Task<OperationResult> DeleteCharacterServiceAsync(Guid playerId, Guid characterId)
    {
        TeamDbo? team = await GetTeamInternalAsync(playerId);
        if (team == null)
        {
            return OperationResult.NotFound();
        }

        CharacterDbo? character = await team.Characters.SingleOrDefaultAsync(c => c.Id == characterId);
        if (character == null)
        {
            return OperationResult.NotFound();
        }

        character.Remove();
        return OperationResult.NoContent();
    }

    static async Task<TeamDbo?> GetTeamInternalAsync(Guid playerId) =>
        (await Query.All<TeamDbo>().Where(t => t.PlayerId == playerId).Prefetch(t => t.Characters).ExecuteAsync()).SingleOrDefault();

    static async Task<TeamDbo> GetOrCreateTeamAsync(Guid playerId) => await GetTeamInternalAsync(playerId) ?? new TeamDbo(playerId);
}
