using RestAdventure.Core.Maps;

namespace RestAdventure.Core.Characters;

public class GameCharactersState
{
    readonly Dictionary<Guid, Team> _teams = [];
    readonly Dictionary<Guid, Character> _characters = [];

    public GameCharactersState(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public IReadOnlyCollection<Team> Teams => _teams.Values;

    public Team CreateTeam(Guid playerId)
    {
        Team team = new(this, playerId);

        _teams[team.Id] = team;

        return team;
    }

    public Team? GetTeam(Guid teamId) => _teams.GetValueOrDefault(teamId);
    public IEnumerable<Team> GetTeamsOfPlayer(Guid playerId) => _teams.Values.Where(t => t.PlayerId == playerId);

    public CharacterCreationResult CreateCharacter(Team team, string name, CharacterClass characterClass, MapLocation? location = null)
    {
        int maxTeamSize = GameState.Settings.MaxTeamSize;
        IEnumerable<Character> characters = GetCharactersInTeam(team);
        if (characters.Count() >= maxTeamSize)
        {
            return new CharacterCreationResult { IsSuccess = false, ErrorMessage = $"reached max team size ({maxTeamSize})" };
        }

        location ??= GameState.Map.Locations.First();

        Character character = new(team, name, characterClass, location);

        _characters[character.Id] = character;

        return new CharacterCreationResult { IsSuccess = true, Character = character };
    }

    public Character? GetCharacter(Team team, Guid characterId)
    {
        Character? character = _characters.GetValueOrDefault(characterId);
        if (character == null || character.Team.Id != team.Id)
        {
            return null;
        }

        return character;
    }

    public IEnumerable<Character> GetCharactersInTeam(Team team) => _characters.Values.Where(c => c.Team == team);
    public IEnumerable<Character> GetCharactersAtLocation(MapLocation location) => _characters.Values.Where(c => c.Location == location);

    public void DeleteCharacter(Team team, Guid characterId)
    {
        if (!_characters.TryGetValue(characterId, out Character? character) || character.Team != team)
        {
            throw new InvalidOperationException($"Could not find character {characterId}");
        }

        _characters.Remove(characterId);
    }
}

public static class GameCharactersStateExtensions
{
    public static Team RequireTeam(this GameCharactersState state, Guid teamId) => state.GetTeam(teamId) ?? throw new InvalidOperationException($"Could not find team {teamId}");

    public static Character RequireCharacter(this GameCharactersState state, Guid teamId, Guid characterId)
    {
        Team team = state.RequireTeam(teamId);
        return state.RequireCharacter(team, characterId);
    }

    public static Character RequireCharacter(this GameCharactersState state, Team team, Guid characterId) =>
        state.GetCharacter(team, characterId) ?? throw new InvalidOperationException($"Could not find character {characterId}");
}
