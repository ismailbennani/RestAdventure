import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, Subject, catchError, debounceTime, delay, first, map, of, switchMap, takeUntil, tap } from 'rxjs';
import { AdminGameApiClient } from '../../../api/admin-api-client.generated';
import { GameSettings, GameState } from '../../../api/game-api-client.generated';

@Injectable({
  providedIn: 'root',
})
export class GameService {
  static readonly RECONNECTION_DELAY: number = 5000;
  static readonly PAUSE_REFRESH_PERIOD: number = 5000;
  static readonly TICKING_DELAY: number = 100;

  private refreshingInternal: boolean = false;
  private refreshSubject: Subject<boolean> = new Subject<boolean>();
  private refreshedSubject: Subject<void> = new Subject<void>();

  private gameSettings: GameSettings | undefined;

  private connectedInternal: boolean = false;
  private connectedSubject: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  private stateInternal: GameState | undefined;
  private stateSubject: ReplaySubject<GameState> = new ReplaySubject<GameState>(1);

  private maxScheduledRefresh: number = 0;

  constructor(private adminGameApiClient: AdminGameApiClient) {}

  public get connected(): boolean {
    return this.connectedInternal;
  }

  public get connected$(): Observable<boolean> {
    return this.connectedSubject;
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
        tap(() => {
          this.refreshingInternal = false;
          this.refreshedSubject.next();
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
      .pipe(switchMap(_ => this.refreshNow()))
      .subscribe();
  }

  pause() {
    this.adminGameApiClient
      .stopSimulation()
      .pipe(switchMap(_ => this.refreshNow()))
      .subscribe();
  }

  step() {
    this.adminGameApiClient
      .tickNow()
      .pipe(switchMap(_ => this.refreshNow()))
      .subscribe();
  }

  refreshNow(force: boolean = false): Observable<void> {
    this.refreshSubject.next(force);
    return this.refreshedSubject.pipe(first());
  }

  refreshIn(delayMs: number) {
    this.registerPlannedRefreshTime(Date.now() + delayMs);

    of({})
      .pipe(
        delay(delayMs),
        takeUntil(this.refreshSubject),
        switchMap(() => this.refreshNow()),
      )
      .subscribe();
  }

  refreshAt(date: Date) {
    this.registerPlannedRefreshTime(date.getTime());

    of({})
      .pipe(
        delay(date),
        takeUntil(this.refreshSubject),
        switchMap(() => this.refreshNow()),
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

    if (this.maxScheduledRefresh > now) {
      return;
    }

    this.refreshIn(GameService.TICKING_DELAY);
  }

  private registerPlannedRefreshTime(time: number) {
    if (this.maxScheduledRefresh < time) {
      this.maxScheduledRefresh = time;
    }
  }
}
