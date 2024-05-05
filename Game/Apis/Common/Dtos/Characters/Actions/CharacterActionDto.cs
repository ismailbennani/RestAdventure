using System.Text.Json.Serialization;
using RestAdventure.Core.Gameplay.Actions;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     An action performed by a character
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(CharacterMoveToLocationActionDto), "move-to-location")]
public class CharacterActionDto
{
}

static class CharacterActionMappingExtensions
{
    public static CharacterActionDto ToDto(this CharacterAction action) =>
        action switch
        {
            CharacterMoveToLocationAction moveToLocationAction => moveToLocationAction.ToDto(),
            _ => new CharacterActionDto()
        };
}
