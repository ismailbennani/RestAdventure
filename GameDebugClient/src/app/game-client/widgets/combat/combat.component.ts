import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CombatInstance, EntityMinimal } from '../../../../api/game-api-client.generated';
import { ProgressionBarComponent } from '../../../common/spinner/progression-bar/progression-bar.component';

@Component({
  selector: 'app-combat',
  standalone: true,
  templateUrl: './combat.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ProgressionBarComponent],
})
export class CombatComponent {
  @Input({ required: true })
  public get combat(): CombatInstance {
    return this._combat;
  }
  public set combat(value: CombatInstance) {
    this._combat = value;
    this.attackers = [...value.attackers].reverse();
    this.defenders = value.defenders;
  }
  private _combat: CombatInstance = null!;

  protected attackers: EntityMinimal[] = [];
  protected defenders: EntityMinimal[] = [];
}
