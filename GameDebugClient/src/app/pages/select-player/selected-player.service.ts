import { Injectable } from '@angular/core';
import { Player } from '../../../api/admin-api-client.generated';

@Injectable({
  providedIn: 'root',
})
export class SelectedPlayerService {
  static LOCAL_STORAGE_KEY = 'player';

  private player: Player | undefined;

  set(player: Player) {
    this.player = player;
    this.savePreferredPlayer(player.id);
  }

  get() {
    if (!this.player) {
      throw new Error('No player selected');
    }

    return this.player;
  }

  loadPreferredPlayer(): string | undefined {
    return localStorage.getItem(SelectedPlayerService.LOCAL_STORAGE_KEY) ?? undefined;
  }

  savePreferredPlayer(playerId: string) {
    localStorage.setItem(SelectedPlayerService.LOCAL_STORAGE_KEY, playerId);
  }

  clear() {
    localStorage.removeItem(SelectedPlayerService.LOCAL_STORAGE_KEY);
  }
}
