import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReplaySubject, combineLatest, forkJoin, map, of, switchMap } from 'rxjs';
import { ILocationMinimal, LocationMinimal, Player } from '../../../../api/admin-api-client.generated';
import {
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
import { PlayersService } from '../../services/players/players.service';
import { TeamService } from '../../services/team/team.service';
import { CharacterHistoryComponent } from '../../widgets/character-history/character-history.component';
import { CharacterComponent } from '../../widgets/character/character.component';
import { InventoryComponent } from '../../widgets/inventory/inventory.component';
import { JobsComponent } from '../../widgets/jobs/jobs.component';

@Component({
  selector: 'app-character-page',
  templateUrl: './character-page.component.html',
  standalone: true,
  imports: [CommonModule, SpinnerComponent, InventoryComponent, CharacterComponent, CharacterHistoryComponent, JobsComponent],
})
export class CharacterPageComponent implements OnInit {
  protected loading: boolean = false;
  protected characterId: string | undefined;
  protected character: TeamCharacter | undefined;
  protected accessibleLocations: LocationMinimal[] = [];
  protected entitiesWithInteractions: EntityWithInteractions[] = [];

  private player: Player | undefined;
  private team: Team | undefined;
  private refreshSubject: ReplaySubject<void> = new ReplaySubject<void>(1);

  constructor(
    private route: ActivatedRoute,
    private currentPageService: CurrentPageService,
    private gameService: GameService,
    private playersService: PlayersService,
    private teamService: TeamService,
    private charactersApiClient: TeamCharactersApiClient,
    private charactersActionsApiClient: TeamCharactersActionsApiClient,
  ) {}

  ngOnInit(): void {
    combineLatest({
      characterId: this.route.paramMap.pipe(map(paramMap => paramMap.get('character-id') ?? undefined)),
      player: this.playersService.selected$,
      team: this.teamService.team$,
    }).subscribe(({ characterId, player, team }) => {
      this.characterId = characterId;
      this.player = player;
      this.team = team;
      this.refreshSubject.next();
    });

    this.loading = true;
    this.refreshSubject
      .pipe(
        switchMap(() => {
          if (!this.characterId || !this.team) {
            this.loading = true;
            return of(undefined);
          }

          this.character = this.team?.characters.find(c => c.id === this.characterId);

          if (!this.character && this.team && this.team.characters.length > 0) {
            this.currentPageService.openCharacter(this.team.characters[0]);
            return of(undefined);
          }

          if (!this.character) {
            this.currentPageService.openHome();
            return of(undefined);
          }

          this.loading = false;

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

  moveToLocation(location: LocationMinimal) {
    if (!this.character) {
      return;
    }

    this.charactersActionsApiClient
      .moveToLocation(this.character.id, location.id)
      .pipe(switchMap(_ => this.gameService.refreshNow(true)))
      .subscribe();
  }

  interact(entity: EntityWithInteractions, interaction: InteractionMinimal) {
    if (!this.character) {
      return;
    }

    this.charactersActionsApiClient
      .interact(this.character.id, entity.id, interaction.name)
      .pipe(switchMap(_ => this.gameService.refreshNow(true)))
      .subscribe();
  }

  plansToMove() {
    if (!this.character?.plannedAction) {
      return false;
    }

    return this.character.plannedAction instanceof CharacterMoveToLocationAction;
  }

  plansToMoveToLocation(location: ILocationMinimal) {
    if (!this.character?.plannedAction || !(this.character.plannedAction instanceof CharacterMoveToLocationAction)) {
      return false;
    }

    return this.character.plannedAction.location.id == location.id;
  }

  plansToInteract() {
    if (!this.character?.plannedAction) {
      return false;
    }

    return this.character.plannedAction instanceof CharacterInteractWithEntityAction;
  }

  plansToInteractWithEntity(entity: EntityWithInteractions) {
    if (!this.character?.plannedAction || !(this.character.plannedAction instanceof CharacterInteractWithEntityAction)) {
      return false;
    }

    return this.character.plannedAction.target.id == entity.id;
  }

  plansToPerformInteraction(entity: EntityWithInteractions, interaction: InteractionMinimal) {
    if (!this.character?.plannedAction || !(this.character.plannedAction instanceof CharacterInteractWithEntityAction)) {
      return false;
    }

    return this.character.plannedAction.target.id == entity.id && this.character.plannedAction.interaction.name == interaction.name;
  }

  isPerformingInteraction(entity: EntityWithInteractions, interaction: InteractionMinimal) {
    if (!this.character?.currentInteraction) {
      return false;
    }

    return this.character.currentInteraction.target.id == entity.id && this.character.currentInteraction.interaction.name == interaction.name;
  }

  deleteCharacter() {
    if (!this.characterId) {
      throw new Error('Character id not found');
    }

    this.charactersApiClient
      .deleteCharacter(this.characterId)
      .pipe(switchMap(() => this.gameService.refreshNow(true)))
      .subscribe();
  }
}
