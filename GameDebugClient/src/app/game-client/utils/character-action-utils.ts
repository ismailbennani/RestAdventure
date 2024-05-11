import { Action, HarvestAction, JoinPveCombatAction, MoveAction, StartPveCombatAction } from '../../../api/game-api-client.generated';

export class CharacterActionUtils {
  static toString(action: Action) {
    if (action instanceof MoveAction) {
      return `move to ${action.location.positionX}, ${action.location.positionY} (${action.location.area.name})`;
    }

    if (action instanceof HarvestAction) {
      return `harvest ${action.target.staticObject.name} (${action.name})`;
    }

    if (action instanceof StartPveCombatAction) {
      return `start combat against ${action.monsterGroup.monsters.map(a => a.species.name).join(', ')}`;
    }

    if (action instanceof JoinPveCombatAction) {
      return `join combat ${action.combat.attackers.map(a => a.name).join(', ')} vs. ${action.monsterGroup.monsters.map(a => a.species.name).join(', ')}`;
    }

    return '???';
  }
}
