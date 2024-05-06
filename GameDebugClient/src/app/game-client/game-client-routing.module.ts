import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GameClientComponent } from './game-client.component';

const routes: Routes = [{ path: '', component: GameClientComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GameClientRoutingModule {}
