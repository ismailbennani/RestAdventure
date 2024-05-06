import { Component } from '@angular/core';
import { catchError, finalize, of, tap } from 'rxjs';
import { CharacterClass, CreateCharacterRequest, TeamCharactersApiClient } from '../../../../api/game-api-client.generated';
import { SpinnerComponent } from '../../../common/spinner/spinner.component';
import { CurrentPageService } from '../../services/current-page.service';
import { GameService } from '../../services/game.service';
import { PlayersService } from '../../services/players/players.service';
import { TeamService } from '../../services/team/team.service';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  standalone: true,
  imports: [SpinnerComponent],
})
export class TeamComponent {
  protected inCreation: boolean = false;
  protected creating: boolean = false;

  protected characterClasses: { value: CharacterClass; display: string }[] = [
    {
      value: CharacterClass.Knight,
      display: 'Knight',
    },
    {
      value: CharacterClass.Mage,
      display: 'Mage',
    },
    {
      value: CharacterClass.Scout,
      display: 'Scout',
    },
    {
      value: CharacterClass.Dealer,
      display: 'Dealer',
    },
  ];

  constructor(
    protected currentPageService: CurrentPageService,
    protected gameService: GameService,
    protected playersService: PlayersService,
    protected teamService: TeamService,
    protected teamCharactersApiClient: TeamCharactersApiClient,
  ) {}

  createCharacter(name: string, cls: string) {
    this.creating = true;
    this.teamCharactersApiClient
      .createCharacter(new CreateCharacterRequest({ name, class: cls as CharacterClass }))
      .pipe(
        tap(character => {
          this.gameService.refreshNow(true);
          this.currentPageService.openCharacter(character);
        }),
        finalize(() => {
          this.creating = false;
          this.inCreation = false;
        }),
        catchError(e => {
          console.error('Error while creating character.', e);
          return of(undefined);
        }),
      )
      .subscribe();
  }
}
