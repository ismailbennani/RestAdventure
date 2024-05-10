import { Routes } from '@angular/router';
import { GameClientComponent } from './game-client.component';
import { BlankPageComponent } from './pages/blank-page/blank-page.component';
import { CharacterPageComponent } from './pages/character-page/character-page.component';
import { MapPageComponent } from './pages/map-page/map-page.component';

export const gameClientRoutes: Routes = [
  {
    path: '',
    component: GameClientComponent,
    children: [
      { path: 'characters/:character-id', component: CharacterPageComponent },
      { path: 'maps', component: MapPageComponent },
      { path: '**', component: BlankPageComponent },
    ],
  },
];
