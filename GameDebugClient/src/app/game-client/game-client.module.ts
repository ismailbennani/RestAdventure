import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SpinnerComponent } from '../common/spinner/spinner.component';
import { GameClientRoutingModule } from './game-client-routing.module';
import { GameClientComponent } from './game-client.component';
import { CreateCharacterComponent } from './widgets/create-character/create-character.component';
import { InventoryComponent } from './widgets/inventory/inventory.component';
import { SimulationComponent } from './widgets/simulation/simulation.component';
import { TeamComponent } from './widgets/team/team.component';

@NgModule({
  declarations: [GameClientComponent, InventoryComponent, CreateCharacterComponent, SimulationComponent, TeamComponent],
  imports: [CommonModule, SpinnerComponent, NgbModule, GameClientRoutingModule],
  exports: [GameClientComponent],
})
export class GameClientModule {}
