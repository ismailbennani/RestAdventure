import { Routes } from '@angular/router';

export const GAME_PATH = 'game';
export const SELECT_PLAYER_ROUTE = 'select-player';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: GAME_PATH,
  },
  {
    path: GAME_PATH,
    loadChildren: () => import('./game-client/game-client.routes').then(m => m.gameClientRoutes),
  },
];
