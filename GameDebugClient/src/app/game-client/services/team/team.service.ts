import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, catchError, combineLatest, filter, map, of, switchMap } from 'rxjs';
import { Team, TeamApiClient } from '../../../../api/game-api-client.generated';
import { GameService } from '../game.service';
import { PlayersService } from '../players/players.service';

@Injectable({
  providedIn: 'root',
})
export class TeamService {
  private teamInternal: Team | undefined;
  private teamSubject: ReplaySubject<Team> = new ReplaySubject<Team>(1);

  constructor(gameService: GameService, playersService: PlayersService, teamApiClient: TeamApiClient) {
    combineLatest({ state: gameService.state$, player: playersService.selected$ })
      .pipe(
        filter(result => gameService.connected && !!result.player),
        switchMap(() => teamApiClient.getTeam()),
        map(team => {
          this.teamInternal = team;
          this.teamSubject.next(team);
        }),
        catchError(e => {
          console.error('Error while fetching team.', e);
          return of({});
        }),
      )
      .subscribe();
  }

  public get team(): Team | undefined {
    return this.teamInternal;
  }

  public get team$(): Observable<Team | undefined> {
    return this.teamSubject.asObservable();
  }
}
