using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Content;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Characters.Services;
using RestAdventure.Core.History.Entities;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Characters;
using RestAdventure.Game.Apis.Common.Dtos.History;
using RestAdventure.Game.Apis.Common.Dtos.Queries;
using RestAdventure.Game.Apis.GameApi.Controllers.Requests;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Errors;
using RestAdventure.Kernel.Queries;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Characters operations
/// </summary>
[Route("game/team/characters")]
[OpenApiTag("Team")]
public class CharactersController : GameApiController
{
    readonly GameService _gameService;
    readonly CharactersService _charactersService;

    /// <summary>
    /// </summary>
    public CharactersController(GameService gameService, CharactersService charactersService)
    {
        _gameService = gameService;
        _charactersService = charactersService;
    }

    /// <summary>
    ///     Create character
    /// </summary>
    [HttpPost]
    [ProducesResponseType<TeamCharacterDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamCharacterDto>> CreateCharacterAsync(CreateCharacterRequestDto request)
    {
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();

        Player player = ControllerContext.RequirePlayer(state);

        CharacterClassId clsId = new(request.ClassId);
        CharacterClass? cls = content.Characters.Classes.Get(clsId);
        if (cls == null)
        {
            return NotFound();
        }

        Maybe<Character> character = await _charactersService.CreateCharacterAsync(player, request.Name, cls);

        if (!character.Success)
        {
            return Problem($"Could not create character: {character.WhyNot}", statusCode: StatusCodes.Status400BadRequest);
        }

        return character.Value.ToDto();
    }

    /// <summary>
    ///     Get character
    /// </summary>
    [HttpGet("{characterGuid:guid}")]
    [ProducesResponseType<TeamCharacterDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<TeamCharacterDto> GetCharacter(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();

        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);
        if (character == null || character.Player != player)
        {
            return NotFound();
        }

        return character.ToDto();
    }

    /// <summary>
    ///     Get character history
    /// </summary>
    [HttpGet("{characterGuid:guid}/history")]
    [ProducesResponseType<SearchResultDto<CharacterHistoryEntryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<SearchResultDto<CharacterHistoryEntryDto>> SearchCharacterHistory(Guid characterGuid, [FromQuery] SearchRequestDto request)
    {
        GameState state = _gameService.RequireGameState();

        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);
        if (character == null || character.Player != player)
        {
            return NotFound();
        }

        IEnumerable<EntityHistoryEntry> allEntries = state.History.Character(character.Id).OrderByDescending(he => he.Tick);

        return Search.Paginate(allEntries, request.ToPaginationParameters()).ToDto(c => c.ToDto());
    }

    /// <summary>
    ///     Delete character
    /// </summary>
    [HttpDelete("{characterGuid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCharacterAsync(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);
        if (character == null || character.Player != player)
        {
            return NotFound();
        }

        await state.Entities.DestroyAsync(character);

        return NoContent();
    }
}
