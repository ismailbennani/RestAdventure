import { Component, Input } from '@angular/core';
import { Team, TeamCharacter } from '../../../../api/game-api-client.generated';
import { SpinnerComponent } from '../../../common/spinner/spinner.component';
import { CurrentPageService } from '../../services/current-page.service';
import { CreateCharacterComponent } from '../create-character/create-character.component';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  standalone: true,
  imports: [SpinnerComponent, CreateCharacterComponent],
})
export class TeamComponent {
  @Input({ required: true }) team: Team = null!;
  @Input() slots: number | undefined;

  protected inCreation: boolean = false;

  constructor(protected currentPageService: CurrentPageService) {}

  protected onCharacterCreated(character: TeamCharacter) {
    this.currentPageService.openCharacter(character);
    this.inCreation = false;
  }
}
