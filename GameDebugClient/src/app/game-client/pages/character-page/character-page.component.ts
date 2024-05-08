import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReplaySubject, combineLatest, forkJoin, map, of, switchMap, tap } from 'rxjs';
import { LocationMinimal } from '../../../../api/admin-api-client.generated';
import {
  Action,
  CombatInPreparation,
  CombatInstance,
  CombatSide,
  CombatsApiClient,
  HarvestableEntity,
  PveCombatAction,
  Team,
  TeamCharacter,
  TeamCharactersApiClient,
} from '../../../../api/game-api-client.generated';
import { SpinnerComponent } from '../../../common/spinner/spinner.component';
import { CurrentPageService } from '../../services/current-page.service';
import { GameService } from '../../services/game.service';
import { TeamService } from '../../services/team/team.service';
import { CharacterActionUtils } from '../../utils/character-action-utils';
import { CharacterHarvestsComponent } from '../../widgets/character-harvests/character-harvests.component';
import { CharacterHistoryComponent } from '../../widgets/character-history/character-history.component';
import { CharacterMonstersComponent } from '../../widgets/character-monsters/character-monsters.component';
import { CharacterMovementsComponent } from '../../widgets/character-movements/character-movements.component';
import { CharacterComponent } from '../../widgets/character/character.component';
import { CombatInPreparationComponent } from '../../widgets/combat-in-preparation/combat-in-preparation.component';
import { CombatComponent } from '../../widgets/combat/combat.component';
import { InventoryComponent } from '../../widgets/inventory/inventory.component';
import { JobsComponent } from '../../widgets/jobs/jobs.component';

@Component({
  selector: 'app-character-page',
  templateUrl: './character-page.component.html',
  standalone: true,
  imports: [
    CommonModule,
    SpinnerComponent,
    InventoryComponent,
    CharacterComponent,
    CharacterHistoryComponent,
    JobsComponent,
    CombatComponent,
    CombatInPreparationComponent,
    CharacterMovementsComponent,
    CharacterHarvestsComponent,
    CharacterMonstersComponent,
  ],
})
export class CharacterPageComponent implements OnInit {
  protected loading: boolean = false;
  protected characterId: string | undefined;
  protected character: TeamCharacter | undefined;
  protected accessibleLocations: LocationMinimal[] = [];
  protected harvestableEntities: HarvestableEntity[] = [];
  protected combats: CombatInstance[] = [];
  protected combatsInPreparation: CombatInPreparation[] = [];
  protected selectedCombat: CombatInstance | undefined;
  protected selectedCombatInPreparation: CombatInPreparation | undefined;

  private team: Team | undefined;
  private refreshSubject: ReplaySubject<void> = new ReplaySubject<void>(1);

  constructor(
    private route: ActivatedRoute,
    private currentPageService: CurrentPageService,
    private gameService: GameService,
    private teamService: TeamService,
    private charactersApiClient: TeamCharactersApiClient,
    private combatsApiClient: CombatsApiClient,
  ) {}

  ngOnInit(): void {
    combineLatest({
      characterId: this.route.paramMap.pipe(map(paramMap => paramMap.get('character-id') ?? undefined)),
      team: this.teamService.team$,
    }).subscribe(({ characterId, team }) => {
      this.characterId = characterId;
      this.team = team;
      this.refreshSubject.next();
    });

    this.loading = true;
    this.refreshSubject
      .pipe(
        tap(() => {
          if (!this.characterId || !this.team) {
            this.loading = true;
            return;
          }

          if (this.character?.id !== this.characterId) {
            this.clearStateAfterCharacterChanged();
          }

          this.character = this.team?.characters.find(c => c.id === this.characterId);

          if (!this.character && this.team && this.team.characters.length > 0) {
            this.currentPageService.openCharacter(this.team.characters[0]);
            return;
          }

          if (!this.character) {
            this.currentPageService.openHome();
            return;
          }

          this.loading = false;
        }),
        switchMap(() => {
          if (!this.character) {
            return of(undefined);
          }

          return forkJoin({
            combats: this.combatsApiClient.getCombats(this.character.id),
            combatsInPreparation: this.combatsApiClient.getCombatsInPreparation(this.character.id),
          });
        }),
        map(result => {
          this.combats = result?.combats ?? [];
          this.combatsInPreparation = result?.combatsInPreparation ?? [];

          this.autoSelectCombatAfterRefresh();
        }),
      )
      .subscribe();
  }

  actionToString(action: Action) {
    return CharacterActionUtils.toString(action);
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

  selectCombat(combat: CombatInstance) {
    this.selectedCombat = combat;
    this.selectedCombatInPreparation = undefined;
  }

  selectCombatInPreparation(combatInPreparation: CombatInPreparation) {
    this.selectedCombatInPreparation = combatInPreparation;
    this.selectedCombat = undefined;
  }

  unselectCombat() {
    this.selectedCombat = undefined;
    this.selectedCombatInPreparation = undefined;
  }

  joinCombat(combat: CombatInPreparation) {
    if (!this.character) {
      return;
    }

    this.combatsApiClient
      .joinCombatInPreparation(this.character.id, combat.id, CombatSide.Attackers)
      .pipe(switchMap(() => this.gameService.refreshNow(true)))
      .subscribe();
  }

  plansToJoinCombat(combat: CombatInPreparation) {
    return this.character?.plannedAction && this.isCombatAction(this.character.plannedAction, combat);
  }

  isInCombat(combat: CombatInPreparation) {
    return this.character?.ongoingAction && this.isCombatAction(this.character.ongoingAction, combat);
  }

  private isCombatAction(action: Action, combat: CombatInPreparation | CombatInstance) {
    return action instanceof PveCombatAction && action.combatId === combat.id;
  }

  private autoSelectCombatAfterRefresh() {
    const currentSelection = this.selectedCombat?.id ?? this.selectedCombatInPreparation?.id;
    if (currentSelection) {
      const combat = this.combats.find(c => c.id === currentSelection);
      if (combat) {
        this.selectCombat(combat);
        return;
      }

      const combatInPreparation = this.combatsInPreparation.find(c => c.id === currentSelection);
      if (combatInPreparation) {
        this.selectCombatInPreparation(combatInPreparation);
        return;
      }
    }

    const combatInvolvingCharacter = this.combats.find(c => c.attackers.some(e => e.id === this.characterId) || c.defenders.some(e => e.id === this.characterId));
    if (combatInvolvingCharacter) {
      this.selectCombat(combatInvolvingCharacter);
      return;
    }

    const combatInPreparationInvolvingCharacter = this.combatsInPreparation.find(
      c => c.attackers.some(e => e.id === this.characterId) || c.defenders.some(e => e.id === this.characterId),
    );
    if (combatInPreparationInvolvingCharacter) {
      this.selectCombatInPreparation(combatInPreparationInvolvingCharacter);
      return;
    }

    if (this.combats.length > 0) {
      this.selectCombat(this.combats[0]);
      return;
    }

    if (this.combatsInPreparation.length > 0) {
      this.selectCombatInPreparation(this.combatsInPreparation[0]);
      return;
    }

    this.unselectCombat();
  }

  private clearStateAfterCharacterChanged() {
    this.unselectCombat();
  }
}
