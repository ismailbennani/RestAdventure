import { Routes } from '@angular/router';
import { GameClientComponent } from './pages/game-client/game-client.component';
import { playerSelectedGuard } from './pages/select-player/player-selected.guard';
import { SelectPlayerComponent } from './pages/select-player/select-player.component';

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
    component: GameClientComponent,
  },
  {
    path: SELECT_PLAYER_ROUTE,
    component: SelectPlayerComponent,
  },
];
