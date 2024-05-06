import { Routes } from '@angular/router';
import { playerSelectedGuard } from './pages/select-player/player-selected.guard';
import { SelectPlayerPageComponent } from './pages/select-player/select-player-page.component';

export const GAME_PATH = 'game';
export const SELECT_PLAYER_ROUTE = 'select-player';

export const routeConfig: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'game',
  },
  {
    path: 'game',
    canActivate: [playerSelectedGuard],
    loadChildren: () => import('./game-client/game-client.module').then(m => m.GameClientModule),
  },
  {
    path: SELECT_PLAYER_ROUTE,
    component: SelectPlayerPageComponent,
  },
];
