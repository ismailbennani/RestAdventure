using RestAdventure.Core.Maps;
using RestAdventure.Core.Players;

namespace RestAdventure.Core.Characters;

public class GameCharactersState
{
    readonly Dictionary<TeamId, Team> _teams = [];
    readonly Dictionary<CharacterId, Character> _characters = [];

    public GameCharactersState(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public IReadOnlyCollection<Team> Teams => _teams.Values;

    public Team CreateTeam(PlayerId playerId)
    {
        Team team = new(this, playerId);

        _teams[team.Id] = team;

        return team;
    }

    public Team? GetTeam(TeamId teamId) => _teams.GetValueOrDefault(teamId);
    public IEnumerable<Team> GetTeams(PlayerId playerId) => _teams.Values.Where(t => t.PlayerId == playerId);

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

    public Character? GetCharacter(Team team, CharacterId characterId)
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

    public void DeleteCharacter(Team team, CharacterId characterId)
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
    public static Team RequireTeam(this GameCharactersState state, TeamId teamId) => state.GetTeam(teamId) ?? throw new InvalidOperationException($"Could not find team {teamId}");

    public static Character RequireCharacter(this GameCharactersState state, TeamId teamId, CharacterId characterId)
    {
        Team team = state.RequireTeam(teamId);
        return state.RequireCharacter(team, characterId);
    }

    public static Character RequireCharacter(this GameCharactersState state, Team team, CharacterId characterId) =>
        state.GetCharacter(team, characterId) ?? throw new InvalidOperationException($"Could not find character {characterId}");
}
