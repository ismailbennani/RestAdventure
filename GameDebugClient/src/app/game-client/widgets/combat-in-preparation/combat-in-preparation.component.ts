import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CombatInPreparation } from '../../../../api/game-api-client.generated';
import { ProgressionBarComponent } from '../../../common/spinner/progression-bar/progression-bar.component';

@Component({
  selector: 'app-combat-in-preparation',
  standalone: true,
  templateUrl: './combat-in-preparation.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ProgressionBarComponent],
})
export class CombatInPreparationComponent {
  @Input({ required: true }) combat: CombatInPreparation = null!;
}
