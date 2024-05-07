import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { catchError, finalize, map, of, switchMap, tap } from 'rxjs';
import { AdminGameContentApiClient } from '../../../../api/admin-api-client.generated';
import { CreateCharacterRequest, Team, TeamCharactersApiClient } from '../../../../api/game-api-client.generated';
import { ProgressionBarComponent } from '../../../common/spinner/progression-bar/progression-bar.component';
import { SpinnerComponent } from '../../../common/spinner/spinner.component';
import { CurrentPageService } from '../../services/current-page.service';
import { GameService } from '../../services/game.service';
import { PlayersService } from '../../services/players/players.service';
import { TeamService } from '../../services/team/team.service';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  standalone: true,
  imports: [CommonModule, SpinnerComponent, ProgressionBarComponent],
})
export class TeamComponent implements OnInit {
  protected loadingClasses: boolean = false;
  protected inCreation: boolean = false;
  protected creating: boolean = false;

  protected team: Team | undefined;
  protected characterClasses: { value: string; display: string }[] = [];
  protected computedValues: { [characterId: string]: { healthPercent: number } } = {};

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

    this.teamService.team$.subscribe(team => {
      this.team = team;

      this.computedValues = {};
      if (team) {
        for (const character of team.characters) {
          this.computedValues[character.id] = {
            healthPercent: this.getPercent(character.combat.health, character.combat.maxHealth),
          };
        }
      }
    });
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

  protected getPercent(value: number, maxValue: number) {
    return maxValue == 0 ? 0 : Math.floor((value * 100) / maxValue);
  }
}
