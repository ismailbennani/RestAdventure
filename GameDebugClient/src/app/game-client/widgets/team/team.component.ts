import { Component, OnInit } from '@angular/core';
import { catchError, finalize, map, of, switchMap, tap } from 'rxjs';
import { AdminGameContentApiClient } from '../../../../api/admin-api-client.generated';
import { CreateCharacterRequest, TeamCharactersApiClient } from '../../../../api/game-api-client.generated';
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
export class TeamComponent implements OnInit {
  protected loadingClasses: boolean = false;
  protected inCreation: boolean = false;
  protected creating: boolean = false;

  protected characterClasses: { value: string; display: string }[] = [];

  constructor(
    protected currentPageService: CurrentPageService,
    protected gameService: GameService,
    protected playersService: PlayersService,
    protected teamService: TeamService,
    protected adminGameContentApiClient: AdminGameContentApiClient,
    protected teamCharactersApiClient: TeamCharactersApiClient,
  ) {}

  ngOnInit(): void {
    this.loadingClasses = true;
    this.adminGameContentApiClient
      .searchCharacterClasses(1, 100)
      .pipe(finalize(() => (this.loadingClasses = false)))
      .subscribe(result => (this.characterClasses = result.items.map(c => ({ value: c.id, display: c.name }))));
  }

  createCharacter(name: string, cls: string) {
    this.creating = true;
    this.teamCharactersApiClient
      .createCharacter(new CreateCharacterRequest({ name, classId: cls }))
      .pipe(
        switchMap(character => this.gameService.refreshNow(true).pipe(map(_ => character))),
        tap(character => this.currentPageService.openCharacter(character)),
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
