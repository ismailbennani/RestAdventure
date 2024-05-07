import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, filter, map, switchMap } from 'rxjs';
import { AdminPlayersApiClient, Player } from '../../../../api/admin-api-client.generated';
import { GameService } from '../game.service';

@Injectable({
  providedIn: 'root',
})
export class PlayersService {
  static LOCAL_STORAGE_KEY = 'player';

  private playersInternal: Player[] = [];
  private playersSubject: BehaviorSubject<Player[]> = new BehaviorSubject<Player[]>([]);

  private selectedPlayerInternal: Player | undefined;
  private selectedPlayerSubject: BehaviorSubject<Player | undefined> = new BehaviorSubject<Player | undefined>(undefined);

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
    let playerInternal = this.playersInternal.find(p => p.id === playerId);
    if (!playerInternal && this.playersInternal.length > 0) {
      playerInternal = this.playersInternal[0];
    }

    if (playerInternal) {
      this.selectedPlayerInternal = playerInternal;
      this.selectedPlayerSubject.next(playerInternal);
      localStorage.setItem(PlayersService.LOCAL_STORAGE_KEY, playerId);
    } else {
      console.error('Unknown player.', playerId);
      this.unselect();
    }
  }

  unselect() {
    this.selectedPlayerInternal = undefined;
    this.selectedPlayerSubject.next(undefined);
    localStorage.removeItem(PlayersService.LOCAL_STORAGE_KEY);
  }

  refreshAndSelect(playerId: string): void {
    this.refreshInternal(playerId).subscribe();
  }

  private refreshInternal(playerId?: string): Observable<void> {
    return this.adminPlayersApiClient.getPlayers().pipe(
      map(players => {
        this.playersInternal = players;
        this.playersSubject.next(players);

        if (playerId && this.playersInternal.find(p => p.id == playerId)) {
          this.select(playerId);
          return;
        }

        const preferredPlayerId = localStorage.getItem(PlayersService.LOCAL_STORAGE_KEY);
        if (preferredPlayerId) {
          const preferredPlayer = this.playersInternal.find(p => p.id === preferredPlayerId);
          if (preferredPlayer) {
            this.select(preferredPlayer.id);
            return;
          }
        }

        if (this.playersInternal.length > 0) {
          this.select(this.playersInternal[0].id);
          return;
        }

        this.unselect();
      }),
    );
  }
}
