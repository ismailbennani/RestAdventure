import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { CharacterAction, TeamCharacter } from '../../../../api/game-api-client.generated';
import { ProgressionBarComponent } from '../../../common/spinner/progression-bar/progression-bar.component';
import { CharacterActionUtils } from '../../utils/character-action-utils';

@Component({
  selector: 'app-character',
  standalone: true,
  templateUrl: './character.component.html',
  imports: [CommonModule, ProgressionBarComponent],
})
export class CharacterComponent {
  @Input({ required: true }) character: TeamCharacter = null!;

  protected healthPercent: number = 0;

  protected actionToString(action: CharacterAction) {
    return CharacterActionUtils.toString(action);
  }
}
