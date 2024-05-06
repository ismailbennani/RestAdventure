import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { LocationMinimal, Player } from '../../api/admin-api-client.generated';
import { EntityWithInteractions, GameSettings, GameState, Team, TeamCharacter } from '../../api/game-api-client.generated';
import { SpinnerComponent } from '../common/spinner/spinner.component';
import { SelectedPlayerService } from '../pages/select-player/selected-player.service';
import { SELECT_PLAYER_ROUTE } from '../routes';
import { CurrentPageService } from './services/current-page.service';
import { GameService } from './services/game.service';
import { CreateCharacterComponent } from './widgets/create-character/create-character.component';
import { InventoryComponent } from './widgets/inventory/inventory.component';
import { SimulationComponent } from './widgets/simulation/simulation.component';
import { TeamComponent } from './widgets/team/team.component';

@Component({
  selector: 'app-game-client',
  templateUrl: './game-client.component.html',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NgbDropdownModule, SpinnerComponent, InventoryComponent, CreateCharacterComponent, SimulationComponent, TeamComponent],
  providers: [CurrentPageService],
})
export class GameClientComponent implements OnInit {
  protected settings: GameSettings = new GameSettings();

  protected loading: boolean = false;
  protected player: Player;
  protected characters: (TeamCharacter | undefined)[] = [];
  protected accessibleLocations: { [characterId: string]: LocationMinimal[] } = {};
  protected entitiesWithInteractions: { [characterId: string]: EntityWithInteractions[] } = {};
  protected performingAction: { [characterId: string]: boolean } = {};

  protected get gameState(): GameState | undefined {
    return this.gameService.state;
  }
  protected get team(): Team | undefined {
    return this.gameService.team;
  }

  constructor(
    selectedPlayerService: SelectedPlayerService,
    private gameService: GameService,
    protected currentPageService: CurrentPageService,
    private router: Router,
  ) {
    this.player = selectedPlayerService.get();
  }

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
