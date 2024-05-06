import { Component } from '@angular/core';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-simulation',
  templateUrl: './simulation.component.html',
  standalone: true,
})
export class SimulationComponent {
  constructor(protected gameService: GameService) {}
}
