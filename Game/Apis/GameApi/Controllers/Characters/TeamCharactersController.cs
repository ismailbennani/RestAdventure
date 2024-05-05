using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Characters;
using RestAdventure.Game.Apis.GameApi.Controllers.Characters.Requests;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

/// <summary>
///     Team characters operations
/// </summary>
[Route("game/team/characters")]
[OpenApiTag("Team")]
public class TeamCharactersController : GameApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public TeamCharactersController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Create character
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TeamCharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamCharacterDto>> CreateCharacterAsync(CreateCharacterRequestDto request)
    {
        GameContent content = _gameService.RequireGameContent();
        Location starting = content.Maps.Locations.All.First();

        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        Team team = GetOrCreateTeam(state, player);
        CharacterCreationResult result = await state.Characters.CreateCharacterAsync(team, request.Name, request.Class, starting);

        if (!result.IsSuccess)
        {
            return Problem($"Could not create character: {result.ErrorMessage}", statusCode: StatusCodes.Status400BadRequest);
        }

        return result.Character.ToDto(content);
    }

    /// <summary>
    ///     Delete character
    /// </summary>
    [HttpDelete("{characterGuid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCharacterAsync(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        Team? team = state.Characters.GetTeams(player).FirstOrDefault();
        if (team == null)
        {
            return NotFound();
        }

        CharacterId characterId = new(characterGuid);
        Character? character = state.Characters.GetCharacter(team, characterId);
        if (character == null)
        {
            return NotFound();
        }

        await state.Characters.DeleteCharacterAsync(team, characterId);

        return NoContent();
    }

    static Team GetOrCreateTeam(GameState state, Player player) => state.Characters.GetTeams(player).FirstOrDefault() ?? state.Characters.CreateTeam(player);
}
