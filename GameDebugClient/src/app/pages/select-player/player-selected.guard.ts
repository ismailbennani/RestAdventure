import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { catchError, map, of, tap } from 'rxjs';
import { PlayersApiClient } from '../../../api/admin-api-client.generated';
import { SELECT_PLAYER_ROUTE } from '../../routes';
import { SelectedPlayerService } from './selected-player.service';

export const playerSelectedGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  return selectPreferredPlayer().pipe(
    tap(result => {
      if (!result) {
        router.navigateByUrl(SELECT_PLAYER_ROUTE).then();
      }
    }),
  );
};

const selectPreferredPlayer = () => {
  const selectedPlayerService = inject(SelectedPlayerService);

  const preferredPlayer = selectedPlayerService.loadPreferredPlayer();
  if (!preferredPlayer) {
    return of(false);
  }

  const playersApiClient = inject(PlayersApiClient);

  return playersApiClient.getPlayer(preferredPlayer).pipe(
    map(player => {
      selectedPlayerService.set(player);
      return true;
    }),
    catchError(e => {
      console.error(`Error while loading player ${preferredPlayer}.`, e);
      return of(false);
    }),
  );
};
