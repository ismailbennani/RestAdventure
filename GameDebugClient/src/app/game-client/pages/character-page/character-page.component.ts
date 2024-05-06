import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReplaySubject, forkJoin, map, of, switchMap } from 'rxjs';
import { ILocationMinimal, LocationMinimal } from '../../../../api/admin-api-client.generated';
import {
  CharacterAction,
  CharacterInteractWithEntityAction,
  CharacterMoveToLocationAction,
  EntityWithInteractions,
  InteractionMinimal,
  Team,
  TeamCharacter,
  TeamCharactersActionsApiClient,
  TeamCharactersApiClient,
} from '../../../../api/game-api-client.generated';
import { SpinnerComponent } from '../../../common/spinner/spinner.component';
import { CurrentPageService } from '../../services/current-page.service';
import { GameService } from '../../services/game.service';
import { InventoryComponent } from '../../widgets/inventory/inventory.component';

@Component({
  selector: 'app-character-page',
  templateUrl: './character-page.component.html',
  standalone: true,
  imports: [CommonModule, SpinnerComponent, InventoryComponent],
})
export class CharacterPageComponent implements OnInit {
  protected loading: boolean = false;
  protected character: TeamCharacter | undefined;
  protected accessibleLocations: LocationMinimal[] = [];
  protected entitiesWithInteractions: EntityWithInteractions[] = [];

  private characterId: string | undefined;
  private team: Team | undefined;
  private refreshSubject: ReplaySubject<void> = new ReplaySubject<void>(1);

  constructor(
    private route: ActivatedRoute,
    private currentPageService: CurrentPageService,
    private gameService: GameService,
    private charactersApiClient: TeamCharactersApiClient,
    private charactersActionsApiClient: TeamCharactersActionsApiClient,
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(paramMap => {
      this.characterId = paramMap.get('character-id') ?? undefined;
      this.refreshSubject.next();
    });

    this.gameService.team$.subscribe(team => {
      this.team = team;
      this.refreshSubject.next();
    });

    this.refreshSubject
      .pipe(
        switchMap(() => {
          this.character = this.team?.characters.find(c => c.id === this.characterId);
          if (!this.character) {
            return of(undefined);
          }

          this.currentPageService.setOpenedCharacter(this.character);

          return forkJoin({
            locations: this.charactersActionsApiClient.getAccessibleLocations(this.character.id),
            interactions: this.charactersActionsApiClient.getAvailableInteractions(this.character.id),
          });
        }),
        map(result => {
          this.accessibleLocations = result?.locations ?? [];
          this.entitiesWithInteractions = result?.interactions ?? [];
        }),
      )
      .subscribe();
  }

  moveToLocation(character: TeamCharacter, location: LocationMinimal) {
    this.charactersActionsApiClient
      .moveToLocation(character.id, location.id)
      .pipe(map(_ => this.gameService.refreshNow()))
      .subscribe();
  }

  interact(character: TeamCharacter, entity: EntityWithInteractions, interaction: InteractionMinimal) {
    this.charactersActionsApiClient
      .interact(character.id, entity.id, interaction.id)
      .pipe(map(_ => this.gameService.refreshNow()))
      .subscribe();
  }

  plansToMoveToLocation(character: TeamCharacter, location: ILocationMinimal) {
    if (!character.plannedAction) {
      return false;
    }

    if (!(character.plannedAction instanceof CharacterMoveToLocationAction)) {
      return false;
    }

    return character.plannedAction.location.id == location.id;
  }

  plansToInteractWithEntity(character: TeamCharacter, entity: EntityWithInteractions) {
    if (!character.plannedAction) {
      return false;
    }

    if (!(character.plannedAction instanceof CharacterInteractWithEntityAction)) {
      return false;
    }

    return character.plannedAction.entity.id == entity.id;
  }

  plansToPerformInteraction(character: TeamCharacter, entity: EntityWithInteractions, interaction: InteractionMinimal) {
    if (!character.plannedAction) {
      return false;
    }

    if (!(character.plannedAction instanceof CharacterInteractWithEntityAction)) {
      return false;
    }

    return character.plannedAction.entity.id == entity.id && character.plannedAction.interaction.id == interaction.id;
  }

  deleteCharacter() {
    if (!this.characterId) {
      throw new Error('Character id not found');
    }

    this.charactersApiClient.deleteCharacter(this.characterId).subscribe(() => this.gameService.refreshNow());
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
}
