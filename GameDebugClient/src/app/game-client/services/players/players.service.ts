import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, filter, map, switchMap } from 'rxjs';
import { AdminPlayersApiClient, Player } from '../../../../api/admin-api-client.generated';
import { GameService } from '../game.service';

@Injectable({
  providedIn: 'root',
})
export class PlayersService {
  static LOCAL_STORAGE_KEY = 'player';

  private playersInternal: Player[] = [];
  private playersSubject: ReplaySubject<Player[]> = new ReplaySubject<Player[]>(1);

  private selectedPlayerInternal: Player | undefined;
  private selectedPlayerSubject: ReplaySubject<Player | undefined> = new ReplaySubject<Player | undefined>(1);

  constructor(
    gameService: GameService,
    private adminPlayersApiClient: AdminPlayersApiClient,
  ) {
    gameService.connected$
      .pipe(
        filter(Boolean),
        switchMap(() => this.refreshInternal()),
      )
      .subscribe();
  }

  public get players(): Player[] {
    return this.playersInternal;
  }

  public get players$(): Observable<Player[]> {
    return this.playersSubject.asObservable();
  }

  public get selected(): Player | undefined {
    return this.selectedPlayerInternal;
  }

  public get selected$(): Observable<Player | undefined> {
    return this.selectedPlayerSubject.asObservable();
  }

  select(playerId: string) {
    const playerInternal = this.playersInternal.find(p => p.id === playerId);
    if (!playerInternal) {
      console.error('Unknown player.', playerId);
      this.selectedPlayerInternal = undefined;
      this.selectedPlayerSubject.next(undefined);
      localStorage.removeItem(PlayersService.LOCAL_STORAGE_KEY);
    } else {
      this.selectedPlayerInternal = playerInternal;
      this.selectedPlayerSubject.next(playerInternal);
      localStorage.setItem(PlayersService.LOCAL_STORAGE_KEY, playerId);
    }
  }

  refreshAndSelect(playerId: string): void {
    this.refreshInternal().subscribe(() => this.select(playerId));
  }

  private refreshInternal(): Observable<void> {
    return this.adminPlayersApiClient.getPlayers().pipe(
      map(players => {
        this.playersInternal = players;
        this.playersSubject.next(players);

        const preferredPlayerId = localStorage.getItem(PlayersService.LOCAL_STORAGE_KEY);
        if (preferredPlayerId) {
          const preferredPlayer = this.playersInternal.find(p => p.id === preferredPlayerId);
          if (preferredPlayer) {
            this.select(preferredPlayer.id);
          }
        }
      }),
    );
  }
}
