using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Characters.Services;
using RestAdventure.Core.History.Entities;
using RestAdventure.Core.Players;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Characters;
using RestAdventure.Game.Apis.Common.Dtos.History.Characters;
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
    [ProducesResponseType<CharacterDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterDto>> CreateCharacterAsync(CreateCharacterRequestDto request)
    {
        GameContent content = _gameService.RequireGameContent();
        Core.Game state = _gameService.RequireGame();

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
    [ProducesResponseType<CharacterDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<CharacterDto> GetCharacter(Guid characterGuid)
    {
        Core.Game state = _gameService.RequireGame();

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
        GameSnapshot state = _gameService.GetLastSnapshot();

        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return NotFound();
        }

        IEnumerable<EntityHistoryEntry> allEntries = state.History.OfType<EntityHistoryEntry>().Where(e => e.EntityId == character.Id).OrderByDescending(he => he.Tick);

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
        Core.Game state = _gameService.RequireGame();
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
