using RestAdventure.Core.Characters.Notifications;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Players;

namespace RestAdventure.Core.Characters;

public class GameCharacters
{
    readonly Dictionary<TeamId, Team> _teams = [];
    readonly Dictionary<CharacterId, Character> _characters = [];

    public GameCharacters(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public IReadOnlyCollection<Team> Teams => _teams.Values;

    public Team CreateTeam(Player player)
    {
        Team team = new(this, player);

        _teams[team.Id] = team;

        return team;
    }

    public Team? GetTeam(TeamId teamId) => _teams.GetValueOrDefault(teamId);
    public IEnumerable<Team> GetTeams(Player player) => _teams.Values.Where(t => t.Player == player);

    public async Task<CharacterCreationResult> CreateCharacterAsync(Team team, string name, CharacterClass characterClass, MapLocation location)
    {
        int maxTeamSize = GameState.Settings.MaxTeamSize;
        IEnumerable<Character> characters = GetCharactersInTeam(team);
        if (characters.Count() >= maxTeamSize)
        {
            return new CharacterCreationResult { IsSuccess = false, ErrorMessage = $"reached max team size (max:{maxTeamSize})" };
        }

        Character character = new(team, name, characterClass, location);
        await GameState.Publisher.Publish(new CharacterMovedToLocation { Character = character, Location = location });

        _characters[character.Id] = character;

        character.Inventory.Changed += (_, args) => GameState.Publisher.Publish(
                new CharacterInventoryChanged
                {
                    Character = character,
                    ItemInstance = args.ItemInstance,
                    OldCount = args.OldCount,
                    NewCount = args.NewCount
                }
            )
            .Wait();

        await GameState.Publisher.Publish(new CharacterCreated { Character = character });

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

    public async Task DeleteCharacterAsync(Team team, CharacterId characterId)
    {
        if (!_characters.TryGetValue(characterId, out Character? character) || character.Team != team)
        {
            throw new InvalidOperationException($"Could not find character {characterId}");
        }

        _characters.Remove(characterId);

        await GameState.Publisher.Publish(new CharacterDeleted { Character = character });
    }
}

public static class GameCharactersStateExtensions
{
    public static Team RequireTeam(this GameCharacters state, TeamId teamId) => state.GetTeam(teamId) ?? throw new InvalidOperationException($"Could not find team {teamId}");

    public static Character RequireCharacter(this GameCharacters state, TeamId teamId, CharacterId characterId)
    {
        Team team = state.RequireTeam(teamId);
        return state.RequireCharacter(team, characterId);
    }

    public static Character RequireCharacter(this GameCharacters state, Team team, CharacterId characterId) =>
        state.GetCharacter(team, characterId) ?? throw new InvalidOperationException($"Could not find character {characterId}");
}
