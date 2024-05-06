import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, finalize, forkJoin, map, of, switchMap, tap } from 'rxjs';
import { AdminGameApiClient, LocationMinimal, Player } from '../../api/admin-api-client.generated';
import {
  CharacterAction,
  CharacterInteractWithEntityAction,
  CharacterMoveToLocationAction,
  EntityWithInteractions,
  GameApiClient,
  GameSettings,
  GameState,
  ILocationMinimal,
  InteractionMinimal,
  Team,
  TeamApiClient,
  TeamCharacter,
  TeamCharactersActionsApiClient,
  TeamCharactersApiClient,
} from '../../api/game-api-client.generated';
import { SelectedPlayerService } from '../pages/select-player/selected-player.service';
import { SELECT_PLAYER_ROUTE } from '../routes';

@Component({
  selector: 'app-game-client',
  templateUrl: './game-client.component.html',
})
export class GameClientComponent implements OnInit {
  protected settings: GameSettings = new GameSettings();

  protected loading: boolean = false;
  protected player: Player;
  protected gameState: GameState | undefined;
  protected team: Team | undefined;
  protected characters: (TeamCharacter | undefined)[] = [];
  protected accessibleLocations: { [characterId: string]: LocationMinimal[] } = {};
  protected entitiesWithInteractions: { [characterId: string]: EntityWithInteractions[] } = {};
  protected performingAction: { [characterId: string]: boolean } = {};

  constructor(
    selectedPlayerService: SelectedPlayerService,
    private adminGameApiClient: AdminGameApiClient,
    private gameApiClient: GameApiClient,
    private teamApiClient: TeamApiClient,
    private teamCharactersApiClient: TeamCharactersApiClient,
    private teamCharactersActionsApiClient: TeamCharactersActionsApiClient,
    private router: Router,
  ) {
    this.player = selectedPlayerService.get();
  }

  ngOnInit(): void {
    this.loading = true;

    this.gameApiClient
      .getGameSettings()
      .pipe(
        switchMap(settings => {
          this.settings = settings;
          return this.refreshCharacters();
        }),
        finalize(() => (this.loading = false)),
      )
      .subscribe();

    this.refreshGameState();
  }

  moveToLocation(character: TeamCharacter, location: ILocationMinimal) {
    this.performingAction[character.id] = true;
    this.teamCharactersActionsApiClient
      .moveToLocation(character.id, location.id)
      .pipe(
        switchMap(_ => this.refreshCharacters()),
        finalize(() => (this.performingAction[character.id] = false)),
      )
      .subscribe();
  }

  interact(character: TeamCharacter, entity: EntityWithInteractions, interaction: InteractionMinimal) {
    this.performingAction[character.id] = true;
    this.teamCharactersActionsApiClient
      .interact(character.id, entity.id, interaction.id)
      .pipe(
        switchMap(_ => this.refreshCharacters()),
        finalize(() => (this.performingAction[character.id] = false)),
      )
      .subscribe();
  }

  characterPlansToMoveToLocation(character: TeamCharacter, location: ILocationMinimal) {
    if (!character.plannedAction) {
      return false;
    }

    if (!(character.plannedAction instanceof CharacterMoveToLocationAction)) {
      return false;
    }

    return character.plannedAction.location.id == location.id;
  }

  characterPlansToInteractWithEntity(character: TeamCharacter, entity: EntityWithInteractions) {
    if (!character.plannedAction) {
      return false;
    }

    if (!(character.plannedAction instanceof CharacterInteractWithEntityAction)) {
      return false;
    }

    return character.plannedAction.entity.id == entity.id;
  }

  characterPlansToPerformInteraction(character: TeamCharacter, entity: EntityWithInteractions, interaction: InteractionMinimal) {
    if (!character.plannedAction) {
      return false;
    }

    if (!(character.plannedAction instanceof CharacterInteractWithEntityAction)) {
      return false;
    }

    return character.plannedAction.entity.id == entity.id && character.plannedAction.interaction.id == interaction.id;
  }

  changePlayer() {
    this.router.navigateByUrl(SELECT_PLAYER_ROUTE);
  }

  protected actionToString(action: CharacterAction) {
    if (action instanceof CharacterMoveToLocationAction) {
      return `move to ${action.location.positionX}, ${action.location.positionY} (${action.location.area.name})`;
    }

    if (action instanceof CharacterInteractWithEntityAction) {
      return `interact with ${action.entity.name}: ${action.interaction.name}`;
    }

    return '???';
  }

  protected addCharacter(character: TeamCharacter, index: number) {
    this.refreshCharacters().subscribe();
  }

  protected deleteCharacter(character: TeamCharacter) {
    this.teamCharactersApiClient
      .deleteCharacter(character.id)
      .pipe(switchMap(() => this.refreshCharacters()))
      .subscribe();
  }

  protected refreshGameState() {
    this.gameApiClient
      .getGameState()
      .pipe(
        tap(state => {
          if (state.tick != this.gameState?.tick) {
            this.refreshCharacters().subscribe();
          }
        }),
      )
      .subscribe(state => {
        this.gameState = state;
        const now = Date.now();
        const nextTick = state.nextTickDate.getTime();

        const toWait = nextTick > now ? nextTick - now : 1000;

        setTimeout(() => this.refreshGameState(), toWait);
      });
  }

  private refreshCharacters(): Observable<unknown> {
    return this.teamApiClient.getTeam().pipe(
      map(team => {
        this.team = team;
        this.characters = new Array(this.settings.maxTeamSize);
        for (let i = 0; i < team.characters.length; i++) {
          this.characters[i] = team.characters[i];
        }

        return team;
      }),
      switchMap(_ => this.refreshAccessibleLocations()),
      switchMap(_ => this.refreshAvailableInteractions()),
    );
  }

  private refreshAccessibleLocations(): Observable<unknown> {
    const characters: TeamCharacter[] = this.characters.filter(c => Boolean(c)).map(c => c as TeamCharacter);
    if (characters.length === 0) {
      return of(void 0);
    }

    return forkJoin(characters.map(c => this.teamCharactersActionsApiClient.getAccessibleLocations(c.id).pipe(map(locations => ({ character: c, locations }))))).pipe(
      map(results => {
        this.accessibleLocations = {};
        for (var result of results) {
          this.accessibleLocations[result.character.id] = result.locations;
        }
      }),
    );
  }

  private refreshAvailableInteractions(): Observable<unknown> {
    const characters: TeamCharacter[] = this.characters.filter(c => Boolean(c)).map(c => c as TeamCharacter);
    if (characters.length === 0) {
      return of(void 0);
    }

    return forkJoin(characters.map(c => this.teamCharactersActionsApiClient.getAvailableInteractions(c.id).pipe(map(entity => ({ character: c, entity }))))).pipe(
      map(results => {
        this.entitiesWithInteractions = {};
        for (var result of results) {
          this.entitiesWithInteractions[result.character.id] = result.entity;
        }
      }),
    );
  }
}
