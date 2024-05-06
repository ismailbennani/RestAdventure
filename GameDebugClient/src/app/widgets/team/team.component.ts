import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { Team } from '../../../api/game-api-client.generated';
import { SpinnerComponent } from '../../common/spinner/spinner.component';
import { CreateCharacterComponent } from '../create-character/create-character.component';

@Component({
  selector: 'app-team',
  standalone: true,
  templateUrl: './team.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, SpinnerComponent, CreateCharacterComponent],
})
export class TeamComponent {
  @Input({ required: true }) team: Team = null!;
  @Input() slots: number | undefined;

  protected inCreation: boolean = false;
}
