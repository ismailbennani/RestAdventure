import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, forkJoin, map, switchMap } from 'rxjs';
import {
  Action,
  ArchivedCombat,
  CombatFormationAccessibility,
  CombatFormationOptions,
  CombatInPreparation,
  CombatInstance,
  CombatSide,
  CombatsApiClient,
  CombatsHistoryApiClient,
  CombatsInPreparationApiClient,
  PveCombatAction,
  TeamCharacter,
} from '../../../../api/game-api-client.generated';
import { GameService } from '../../services/game.service';
import { CombatHistoryComponent } from '../combat-history/combat-history.component';
import { CombatInPreparationComponent } from '../combat-in-preparation/combat-in-preparation.component';
import { CombatComponent } from '../combat/combat.component';

@Component({
  selector: 'app-character-combats',
  standalone: true,
  templateUrl: './character-combats.component.html',
  imports: [CommonModule, CombatComponent, CombatInPreparationComponent, CombatHistoryComponent],
})
export class CharacterCombatsComponent implements OnInit {
  @Input({ required: true })
  public get character(): TeamCharacter {
    return this._character;
  }
  public set character(value: TeamCharacter) {
    this._character = value;
    this.characterSubject.next(value);
  }

  protected combats: CombatInstance[] = [];
  protected combatsInPreparation: CombatInPreparation[] = [];
  protected archivedCombats: ArchivedCombat[] = [];
  protected selectedCombat: CombatInstance | undefined;
  protected selectedCombatInPreparation: CombatInPreparation | undefined;
  protected selectedArchivedCombat: ArchivedCombat | undefined;

  protected cachedValues: { [combatId: string]: { attackersDisplay: string; defendersDisplay: string } } = {};

  private _character: TeamCharacter = null!;
  private characterSubject: ReplaySubject<TeamCharacter> = new ReplaySubject<TeamCharacter>(1);

  constructor(
    private gameService: GameService,
    private combatsInPreparationApiClient: CombatsInPreparationApiClient,
    private combatsApiClient: CombatsApiClient,
    private combatsHistoryApiClient: CombatsHistoryApiClient,
  ) {}

  ngOnInit(): void {
    this.characterSubject
      .pipe(
        switchMap(() => {
          return forkJoin({
            combats: this.combatsApiClient.getCombats(this.character.id),
            combatsInPreparation: this.combatsInPreparationApiClient.getCombatsInPreparation(this.character.id),
            archivedCombats: this.combatsHistoryApiClient.searchArchivedCombats(this.character.id),
          });
        }),
        map(result => {
          this.combats = result?.combats ?? [];
          this.combatsInPreparation = result?.combatsInPreparation ?? [];
          this.archivedCombats = result?.archivedCombats.items ?? [];

          this.cachedValues = {};
          this.updateCachedValues(this.combats.map(c => ({ id: c.id, attackers: c.attackers, defenders: c.defenders })));
          this.updateCachedValues(this.combatsInPreparation.map(c => ({ id: c.id, attackers: c.attackers.entities, defenders: c.defenders.entities })));
          this.updateCachedValues(this.archivedCombats.map(c => ({ id: c.id, attackers: c.attackers, defenders: c.defenders })));

          this.autoSelectCombatAfterRefresh();
        }),
      )
      .subscribe();
  }

  selectCombat(combat: CombatInstance) {
    this.selectedCombat = combat;
    this.selectedCombatInPreparation = undefined;
    this.selectedArchivedCombat = undefined;
  }

  selectCombatInPreparation(combatInPreparation: CombatInPreparation) {
    this.selectedCombatInPreparation = combatInPreparation;
    this.selectedCombat = undefined;
    this.selectedArchivedCombat = undefined;
  }

  selectArchivedCombat(archivedCombat: ArchivedCombat) {
    this.selectedCombatInPreparation = undefined;
    this.selectedCombat = undefined;
    this.selectedArchivedCombat = archivedCombat;
  }

  unselectCombat() {
    this.selectedCombat = undefined;
    this.selectedCombatInPreparation = undefined;
    this.selectedArchivedCombat = undefined;
  }

  joinCombat(combat: CombatInPreparation) {
    if (!this.character) {
      return;
    }

    this.combatsInPreparationApiClient
      .joinCombatInPreparation(this.character.id, combat.id, CombatSide.Attackers)
      .pipe(switchMap(() => this.gameService.refreshNow(true)))
      .subscribe();
  }

  plansToJoinCombat(combat: CombatInPreparation) {
    return this.character?.plannedAction && this.isCombatAction(this.character.plannedAction, combat);
  }

  isInCombat(combat: CombatInPreparation | CombatInstance) {
    return this.character?.ongoingAction && this.isCombatAction(this.character.ongoingAction, combat);
  }

  changeAccessibility(combat: CombatInPreparation, accessibility: CombatFormationAccessibility) {
    if (!this.character) {
      return;
    }

    const newOptions = new CombatFormationOptions({ ...combat.attackers.options, accessibility: accessibility });
    this.combatsInPreparationApiClient
      .setCombatInPreparationOptions(this.character.id, combat.id, CombatSide.Attackers, newOptions)
      .pipe(switchMap(() => this.gameService.refreshNow(true)))
      .subscribe();
  }

  private isCombatAction(action: Action, combat: CombatInPreparation | CombatInstance) {
    return action instanceof PveCombatAction && action.combatId === combat.id;
  }

  private autoSelectCombatAfterRefresh() {
    const currentSelection = this.selectedCombat?.id ?? this.selectedCombatInPreparation?.id ?? this.selectedArchivedCombat?.id;
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

      const archivedCombat = this.archivedCombats.find(c => c.id === currentSelection);
      if (archivedCombat) {
        this.selectArchivedCombat(archivedCombat);
        return;
      }
    }

    const combatInvolvingCharacter = this.combats.find(c => c.attackers.some(e => e.id === this.character.id) || c.defenders.some(e => e.id === this.character.id));
    if (combatInvolvingCharacter) {
      this.selectCombat(combatInvolvingCharacter);
      return;
    }

    const combatInPreparationInvolvingCharacter = this.combatsInPreparation.find(
      c => c.attackers.entities.some(e => e.id === this.character.id) || c.defenders.entities.some(e => e.id === this.character.id),
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

    if (this.archivedCombats.length > 0) {
      this.selectArchivedCombat(this.archivedCombats[0]);
      return;
    }

    this.unselectCombat();
  }

  private updateCachedValues(combats: { id: string; attackers: { name: string }[]; defenders: { name: string }[] }[]) {
    for (const combat of combats) {
      this.cachedValues[combat.id] = {
        attackersDisplay: this.computeEntitiesDisplay(combat.attackers),
        defendersDisplay: this.computeEntitiesDisplay(combat.defenders),
      };
    }
  }

  private computeEntitiesDisplay(entities: { name: string }[]) {
    const entitiesWithCount: { [entityName: string]: { name: string; count: number } } = {};
    for (const entity of entities) {
      const existing = entitiesWithCount[entity.name];
      if (existing) {
        existing.count++;
      } else {
        entitiesWithCount[entity.name] = { ...entity, count: 1 };
      }
    }

    return Object.values(entitiesWithCount)
      .sort((e1, e2) => e2.count - e1.count)
      .map(e => (e.count == 1 ? e.name : `${e.count}x ${e.name}`))
      .join(', ');
  }
}
