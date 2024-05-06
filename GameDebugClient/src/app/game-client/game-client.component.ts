import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { LocationMinimal } from '../../api/admin-api-client.generated';
import { EntityWithInteractions, GameSettings, GameState, TeamCharacter } from '../../api/game-api-client.generated';
import { SpinnerComponent } from '../common/spinner/spinner.component';
import { SELECT_PLAYER_ROUTE } from '../routes';
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
  protected settings: GameSettings = new GameSettings();

  protected loading: boolean = false;
  protected characters: (TeamCharacter | undefined)[] = [];
  protected accessibleLocations: { [characterId: string]: LocationMinimal[] } = {};
  protected entitiesWithInteractions: { [characterId: string]: EntityWithInteractions[] } = {};
  protected performingAction: { [characterId: string]: boolean } = {};

  protected get gameState(): GameState | undefined {
    return this.gameService.state;
  }

  constructor(
    private gameService: GameService,
    protected currentPageService: CurrentPageService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.loading = true;

    this.gameService.start().subscribe(settings => (this.settings = settings));
  }

  changePlayer() {
    this.router.navigate([SELECT_PLAYER_ROUTE]);
  }

  protected refresh() {
    this.gameService.refreshNow();
  }
}
