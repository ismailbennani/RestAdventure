import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, map, switchMap, tap } from 'rxjs';
import { StaticObject } from '../../../../api/admin-api-client.generated';
import { Action, Character, HarvestAction, ItemStack, JobsHarvestApiClient } from '../../../../api/game-api-client.generated';
import { GameContentService } from '../../services/game-content.service';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-character-harvests',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './character-harvests.component.html',
})
export class CharacterHarvestsComponent implements OnInit {
  @Input({ required: true })
  public get character(): Character {
    return this._character;
  }
  public set character(value: Character) {
    this._character = value;
    this.characterSubject.next(value);
  }
  private _character: Character = null!;

  protected harvestables: {
    objectId: string;
    objectInstanceId: string;
    name: string;
    actions: { name: string; count: number; expectedHarvest: ItemStack[]; expectedExperience: number; canHarvest: boolean; cannotHarvestReason?: string }[];
  }[] = [];

  private characterSubject: ReplaySubject<Character> = new ReplaySubject<Character>(1);
  private staticObjects: StaticObject[] = [];

  constructor(
    private gameService: GameService,
    private gameContentService: GameContentService,
    private jobsHarvestApiClient: JobsHarvestApiClient,
  ) {}

  ngOnInit(): void {
    this.gameContentService.staticObjects$
      .pipe(
        map(staticObjects => {
          this.staticObjects = staticObjects;
          this.characterSubject.next(this._character);
        }),
      )
      .subscribe();

    this.characterSubject
      .pipe(
        switchMap(character => this.jobsHarvestApiClient.getHarvestables(character.id)),
        tap(harvestables => {
          this.harvestables = [];

          for (const harvestable of harvestables) {
            let harvestableDisplay = this.harvestables.find(h => h.objectId === harvestable.objectId);
            if (!harvestableDisplay) {
              const staticObject = this.staticObjects.find(o => o.id === harvestable.objectId);
              harvestableDisplay = {
                objectId: harvestable.objectId,
                objectInstanceId: harvestable.objectInstanceId,
                name: staticObject?.name ?? harvestable.objectInstanceId,
                actions: [],
              };
              this.harvestables.push(harvestableDisplay);
            }

            for (const action of harvestable.actions) {
              let actionDisplay = harvestableDisplay?.actions.find(
                a => a.name === action.name && a.canHarvest === action.canHarvest && a.cannotHarvestReason === action.whyCannotHarvest,
              );

              if (!actionDisplay) {
                actionDisplay = {
                  name: action.name,
                  count: 0,
                  expectedHarvest: action.expectedHarvest,
                  expectedExperience: action.expectedExperience,
                  canHarvest: action.canHarvest,
                  cannotHarvestReason: action.whyCannotHarvest,
                };
                harvestableDisplay.actions.push(actionDisplay);
              }

              actionDisplay.count += 1;
            }
          }
        }),
      )
      .subscribe();
  }

  harvest(entity: { objectInstanceId: string }, harvest: { name: string; canHarvest: boolean }) {
    if (!this.character || !harvest.canHarvest) {
      return;
    }

    this.jobsHarvestApiClient
      .harvest(this.character.id, entity.objectInstanceId, harvest.name)
      .pipe(switchMap(_ => this.gameService.refreshNow(true)))
      .subscribe();
  }

  plansToHarvest(harvestable?: { objectInstanceId: string }, harvest?: { name: string }) {
    return this.character.plannedAction && this.isHarvestingAction(this.character.plannedAction, harvestable, harvest);
  }

  isHarvesting(harvestable?: { objectInstanceId: string }, harvest?: { name: string }) {
    return this.character.ongoingAction && this.isHarvestingAction(this.character.ongoingAction, harvestable, harvest);
  }

  protected formatExpectedHarvest(items: ItemStack[]) {
    return items.map(s => `${s.count}x ${s.item.name}`).join(', ');
  }

  private isHarvestingAction(action: Action, harvestable?: { objectInstanceId: string }, harvest?: { name: string }) {
    if (!(action instanceof HarvestAction) || !this.harvestables.find(h => h.objectInstanceId === (action as HarvestAction).targetId)) {
      return false;
    }

    if (harvestable && action.targetId !== harvestable.objectInstanceId) {
      return false;
    }

    if (harvest && action.harvest.name !== harvest.name) {
      return false;
    }

    return true;
  }
}
