import { Component } from '@angular/core';
import { TeamCharacter } from '../../../../api/game-api-client.generated';
import { SpinnerComponent } from '../../../common/spinner/spinner.component';
import { CurrentPageService } from '../../services/current-page.service';
import { GameService } from '../../services/game.service';
import { PlayersService } from '../../services/players/players.service';
import { TeamService } from '../../services/team/team.service';
import { CreateCharacterComponent } from '../create-character/create-character.component';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  standalone: true,
  imports: [SpinnerComponent, CreateCharacterComponent],
})
export class TeamComponent {
  protected inCreation: boolean = false;

  constructor(
    protected currentPageService: CurrentPageService,
    protected gameService: GameService,
    protected playersService: PlayersService,
    protected teamService: TeamService,
  ) {}

  protected onCharacterCreated(character: TeamCharacter) {
    this.currentPageService.openCharacter(character);
    this.inCreation = false;
  }
}
