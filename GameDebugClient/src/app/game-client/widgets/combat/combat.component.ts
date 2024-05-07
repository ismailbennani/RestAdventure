import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CombatInstance } from '../../../../api/game-api-client.generated';
import { ProgressionBarComponent } from '../../../common/spinner/progression-bar/progression-bar.component';

@Component({
  selector: 'app-combat',
  standalone: true,
  templateUrl: './combat.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ProgressionBarComponent],
})
export class CombatComponent {
  @Input({ required: true }) combat: CombatInstance = null!;
}
