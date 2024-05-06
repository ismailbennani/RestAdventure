import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { Team } from '../../../../api/game-api-client.generated';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TeamComponent {
  @Input({ required: true }) team: Team = null!;
  @Input() slots: number | undefined;

  protected inCreation: boolean = false;
}
