import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, Subject, catchError, debounceTime, delay, map, of, switchMap, takeUntil, tap } from 'rxjs';
import { AdminGameApiClient } from '../../../api/admin-api-client.generated';
import { GameSettings, GameState, Team, TeamApiClient } from '../../../api/game-api-client.generated';

@Injectable({
  providedIn: 'root',
})
export class GameService {
  static readonly RECONNECTION_DELAY: number = 5000;
  static readonly PAUSE_REFRESH_PERIOD: number = 5000;

  private refreshingInternal: boolean = false;
  private refreshSubject: Subject<boolean> = new Subject<boolean>();

  private gameSettings: GameSettings | undefined;

  private connectedInternal: boolean = false;
  private connectedSubject: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  private stateInternal: GameState | undefined;
  private stateSubject: ReplaySubject<GameState> = new ReplaySubject<GameState>(1);

  private teamInternal: Team | undefined;
  private teamSubject: ReplaySubject<Team> = new ReplaySubject<Team>(1);

  constructor(
    private adminGameApiClient: AdminGameApiClient,
    private teamApiClient: TeamApiClient,
  ) {}

  public get connected(): boolean {
    return this.connectedInternal;
  }

  public get refreshing(): boolean {
    return this.refreshingInternal;
  }

  public get settings(): GameSettings {
    return this.gameSettings!;
  }

  public get state(): GameState | undefined {
    return this.stateInternal;
  }

  public get state$(): Observable<GameState | undefined> {
    return this.stateSubject.asObservable();
  }

  public get team(): Team | undefined {
    return this.teamInternal;
  }

  public get team$(): Observable<Team | undefined> {
    return this.teamSubject.asObservable();
  }

  start() {
    this.refreshSubject
      .pipe(
        debounceTime(10),
        tap(() => (this.refreshingInternal = true)),
        switchMap(force => this.adminGameApiClient.getGameState().pipe(map(state => ({ state, force })))),
        map(result => {
          if (!this.connectedInternal) {
            this.connectedInternal = true;
            this.connectedSubject.next(true);
          }

          if (result.force || result.state.tick !== this.stateInternal?.tick || result.state.paused !== this.stateInternal?.paused) {
            this.stateInternal = result.state;
            this.stateSubject.next(result.state);
          }

          this.scheduleNextRefresh(result.state);
        }),
        catchError(e => {
          if (this.connectedInternal) {
            this.connectedInternal = false;
            this.connectedSubject.next(false);
          }

          console.error(`An error occured while refreshing game state, retry in ${GameService.RECONNECTION_DELAY / 1000}s`, e);

          this.refreshIn(GameService.RECONNECTION_DELAY);

          return of({});
        }),
        tap(() => (this.refreshingInternal = false)),
      )
      .subscribe();

    this.stateSubject
      .pipe(
        switchMap(() => this.teamApiClient.getTeam()),
        map(team => {
          this.teamInternal = team;
          this.teamSubject.next(team);
        }),
      )
      .subscribe();

    return this.adminGameApiClient.getGameSettings().pipe(
      tap(settings => {
        this.gameSettings = settings;
        this.refreshNow();
      }),
    );
  }

  resume() {
    this.adminGameApiClient
      .startSimulation()
      .pipe(map(_ => this.refreshNow()))
      .subscribe();
  }

  pause() {
    this.adminGameApiClient
      .stopSimulation()
      .pipe(map(_ => this.refreshNow()))
      .subscribe();
  }

  step() {
    this.adminGameApiClient
      .tickNow()
      .pipe(map(_ => this.refreshNow()))
      .subscribe();
  }

  refreshNow(force: boolean = false) {
    this.refreshSubject.next(force);
  }

  refreshIn(delayMs: number) {
    of({})
      .pipe(
        delay(delayMs),
        takeUntil(this.refreshSubject),
        map(() => this.refreshNow()),
      )
      .subscribe();
  }

  refreshAt(date: Date) {
    of({})
      .pipe(
        delay(date),
        takeUntil(this.refreshSubject),
        map(() => this.refreshNow()),
      )
      .subscribe();
  }

  private scheduleNextRefresh(state: GameState) {
    if (state.paused || !state.nextTickDate) {
      this.refreshIn(GameService.PAUSE_REFRESH_PERIOD);
      return;
    }

    const nextTickTime = state.nextTickDate.getTime();
    const now = Date.now();

    if (nextTickTime > now) {
      const smallFraction = (nextTickTime - now) * 0.01;
      const nextRefreshDate = new Date(nextTickTime + smallFraction);

      this.refreshAt(nextRefreshDate);
      return;
    }

    this.refreshIn(GameService.PAUSE_REFRESH_PERIOD);
  }
}
