import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { SpinnerComponent } from '../common/spinner/spinner.component';
import { CurrentPageService } from './services/current-page.service';
import { GameService } from './services/game.service';
import { PlayersComponent } from './widgets/players/players.component';
import { SimulationComponent } from './widgets/simulation/simulation.component';
import { TeamComponent } from './widgets/team/team.component';

@Component({
  selector: 'app-game-client',
  templateUrl: './game-client.component.html',
  standalone: true,
  providers: [CurrentPageService],
  imports: [CommonModule, RouterOutlet, NgbDropdownModule, SpinnerComponent, SimulationComponent, TeamComponent, PlayersComponent],
})
export class GameClientComponent implements OnInit {
  constructor(
    private gameService: GameService,
    protected currentPageService: CurrentPageService,
  ) {}

  ngOnInit(): void {
    this.gameService.start().subscribe();
  }
}
