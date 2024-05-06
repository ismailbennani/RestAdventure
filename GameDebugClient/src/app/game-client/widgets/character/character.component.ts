import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CharacterAction, TeamCharacter } from '../../../../api/game-api-client.generated';
import { CharacterActionUtils } from '../../utils/character-action-utils';

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
    return CharacterActionUtils.toString(action);
  }
}
