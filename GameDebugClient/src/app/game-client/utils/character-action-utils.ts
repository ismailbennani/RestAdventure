import { Action, HarvestAction, JoinPveCombatAction, MoveAction, StartPveCombatAction } from '../../../api/game-api-client.generated';

export class CharacterActionUtils {
  static toString(action: Action) {
    if (action instanceof MoveAction) {
      return `move to ${action.location.positionX}, ${action.location.positionY} (${action.location.area.name})`;
    }

    if (action instanceof HarvestAction) {
      return `harvest: ${action.name}`;
    }

    if (action instanceof StartPveCombatAction) {
      return `start combat (against:${action.monsterGroupId})`;
    }

    if (action instanceof JoinPveCombatAction) {
      return `join combat (combat:${action.combatId}, against:${action.monsterGroupId})`;
    }

    return '???';
  }
}
