import { Routes } from '@angular/router';
import { playerSelectedGuard } from './pages/select-player/player-selected.guard';
import { SelectPlayerPageComponent } from './pages/select-player/select-player-page.component';

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
    canActivate: [playerSelectedGuard],
    loadChildren: () => import('./game-client/game-client.routes').then(m => m.gameClientRoutes),
  },
  {
    path: SELECT_PLAYER_ROUTE,
    component: SelectPlayerPageComponent,
  },
];
