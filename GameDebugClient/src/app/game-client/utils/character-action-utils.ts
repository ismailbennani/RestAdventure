import { CharacterAction, CharacterInteractWithEntityAction, CharacterMoveToLocationAction } from '../../../api/game-api-client.generated';

export class CharacterActionUtils {
  static toString(action: CharacterAction) {
    if (action instanceof CharacterMoveToLocationAction) {
      return `move to ${action.location.positionX}, ${action.location.positionY} (${action.location.area.name})`;
    }

    if (action instanceof CharacterInteractWithEntityAction) {
      return `interact with ${action.target.name}: ${action.interaction.name}`;
    }

    return '???';
  }
}
