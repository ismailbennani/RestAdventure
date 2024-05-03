using Microsoft.Extensions.Options;
using RestAdventure.Core.Characters;
using RestAdventure.Game.Apis.GameApi.Controllers.Characters.Requests;
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

    public async Task<TeamDbo> GetTeamAsync(Guid playerId) => await GetOrCreateTeamAsync(playerId);

    public async Task<CharacterCreationResult> CreateCharacterAsync(Guid playerId, CreateCharacterRequestDto request)
    {
        TeamDbo team = await GetOrCreateTeamAsync(playerId);

        long nCharacters = await team.Characters.CountAsync();
        int maxTeamSize = _gameSettings.Value.MaxTeamSize;
        if (nCharacters >= maxTeamSize)
        {
            return new CharacterCreationResult { IsSuccess = false, ErrorMessage = $"reached max team size ({maxTeamSize})" };
        }

        CharacterDbo character = new(team, request.Name, request.Class);

        return new CharacterCreationResult { IsSuccess = true, Character = character };
    }

    public async Task<bool> DeleteCharacterServiceAsync(Guid playerId, Guid characterId)
    {
        TeamDbo? team = await GetTeamInternalAsync(playerId);
        if (team == null)
        {
            return false;
        }

        CharacterDbo? character = await team.Characters.SingleOrDefaultAsync(c => c.Id == characterId);
        if (character == null)
        {
            return false;
        }

        character.Remove();
        return true;
    }

    static async Task<TeamDbo?> GetTeamInternalAsync(Guid playerId) =>
        (await Query.All<TeamDbo>().Where(t => t.PlayerId == playerId).Prefetch(t => t.Characters).ExecuteAsync()).SingleOrDefault();

    static async Task<TeamDbo> GetOrCreateTeamAsync(Guid playerId) => await GetTeamInternalAsync(playerId) ?? new TeamDbo(playerId);
}
