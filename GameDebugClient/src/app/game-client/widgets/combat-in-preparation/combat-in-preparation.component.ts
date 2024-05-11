import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { CombatFormationAccessibility, CombatInPreparation, EntityMinimal } from '../../../../api/game-api-client.generated';
import { ProgressionBarComponent } from '../../../common/spinner/progression-bar/progression-bar.component';

@Component({
  selector: 'app-combat-in-preparation',
  standalone: true,
  templateUrl: './combat-in-preparation.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ProgressionBarComponent],
})
export class CombatInPreparationComponent {
  @Input({ required: true })
  public get combat(): CombatInPreparation {
    return this._combat;
  }
  public set combat(value: CombatInPreparation) {
    this._combat = value;
    this.attackers = [...value.attackers].reverse();
    this.defenders = value.defenders;
  }
  private _combat: CombatInPreparation = null!;

  @Output() public changeAccessibility: EventEmitter<CombatFormationAccessibility> = new EventEmitter<CombatFormationAccessibility>();

  protected updating: boolean = false;
  protected attackers: EntityMinimal[] = [];
  protected defenders: EntityMinimal[] = [];

  protected accessiblityOptions: { id: string; value: CombatFormationAccessibility; displayValue: string }[] = [
    { id: 'everyone', value: CombatFormationAccessibility.Everyone, displayValue: 'Everyone' },
    { id: 'team-only', value: CombatFormationAccessibility.TeamOnly, displayValue: 'Team only' },
  ];

  setAccessibility(accessibility: CombatFormationAccessibility) {
    this.changeAccessibility.next(accessibility);
  }
}
