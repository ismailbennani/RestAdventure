using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters.Requests;

public class CreateCharacterRequestDto
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required CharacterClass Class { get; init; }
}
