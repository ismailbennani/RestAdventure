import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CharacterAction, TeamCharacter } from '../../../../api/game-api-client.generated';
import { ProgressionBarComponent } from '../../../common/spinner/progression-bar/progression-bar.component';
import { CharacterActionUtils } from '../../utils/character-action-utils';

@Component({
  selector: 'app-character',
  standalone: true,
  templateUrl: './character.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ProgressionBarComponent],
})
export class CharacterComponent {
  @Input({ required: true })
  public get character(): TeamCharacter {
    return this._character;
  }
  public set character(value: TeamCharacter) {
    this._character = value;
    this.refreshCachedValues();
  }
  private _character: TeamCharacter = null!;

  protected healthPercent: number = 0;

  protected actionToString(action: CharacterAction) {
    return CharacterActionUtils.toString(action);
  }

  private refreshCachedValues() {
    this.healthPercent = Math.floor((this.character.combat.health * 100) / this.character.combat.maxHealth);
  }
}
