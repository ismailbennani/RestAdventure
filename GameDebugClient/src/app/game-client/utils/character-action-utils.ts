import { Action, HarvestAction, JoinPveCombatAction, MoveAction, PveCombatAction, StartPveCombatAction } from '../../../api/game-api-client.generated';

export class CharacterActionUtils {
  static toString(action: Action) {
    if (action instanceof MoveAction) {
      return `move to ${action.location.positionX}, ${action.location.positionY} (${action.location.area.name})`;
    }

    if (action instanceof HarvestAction) {
      return `harvest ${action.target.staticObject.name} (${action.name})`;
    }

    if (action instanceof StartPveCombatAction) {
      return `start combat ${action.attackers.map(a => a.name).join(', ')} vs. ${action.defenders.map(a => a.name).join(', ')}`;
    }

    if (action instanceof JoinPveCombatAction) {
      return `join combat ${action.attackers.map(a => a.name).join(', ')} vs. ${action.defenders.map(a => a.name).join(', ')}`;
    }

    if (action instanceof PveCombatAction) {
      return `combat ${action.attackers.map(a => a.name).join(', ')} vs. ${action.defenders.map(a => a.name).join(', ')}`;
    }

    return '???';
  }
}
