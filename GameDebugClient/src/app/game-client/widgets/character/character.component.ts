import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CharacterAction, CharacterInteractWithEntityAction, CharacterMoveToLocationAction, TeamCharacter } from '../../../../api/game-api-client.generated';

@Component({
  selector: 'app-character',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './character.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CharacterComponent {
  @Input({ required: true }) character: TeamCharacter = null!;

  protected actionToString(action: CharacterAction) {
    if (action instanceof CharacterMoveToLocationAction) {
      return `move to ${action.location.positionX}, ${action.location.positionY} (${action.location.area.name})`;
    }

    if (action instanceof CharacterInteractWithEntityAction) {
      return `interact with ${action.entity.name}: ${action.interaction.name}`;
    }

    return '???';
  }
}
