import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, switchMap, tap } from 'rxjs';
import { Action, Character, HarvestAction, IHarvestableEntity, IHarvestableEntityHarvest, ItemStack, JobsHarvestApiClient } from '../../../../api/game-api-client.generated';
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

  protected harvestables: (IHarvestableEntity & { count: number })[] = [];

  private _character: Character = null!;
  private characterSubject: ReplaySubject<Character> = new ReplaySubject<Character>(1);

  constructor(
    private gameService: GameService,
    private jobsHarvestApiClient: JobsHarvestApiClient,
  ) {}

  ngOnInit(): void {
    this.characterSubject
      .pipe(
        switchMap(character => this.jobsHarvestApiClient.getHarvestables(character.id)),
        tap(harvestables => {
          this.harvestables = [];
          for (const harvestable of harvestables) {
            let found = false;
            for (const existing of this.harvestables.filter(h => h.name === harvestable.name)) {
              if (existing && harvestable.harvests.every(h => existing.harvests.some(eh => eh.name === h.name))) {
                existing.count++;
                found = true;
                break;
              }
            }

            if (!found) {
              this.harvestables.push({ ...harvestable, count: 1 });
            }
          }
        }),
      )
      .subscribe();
  }

  harvest(entity: IHarvestableEntity, harvest: IHarvestableEntityHarvest) {
    if (!this.character || !harvest.canHarvest) {
      return;
    }

    this.jobsHarvestApiClient
      .harvest(this.character.id, entity.id, harvest.name)
      .pipe(switchMap(_ => this.gameService.refreshNow(true)))
      .subscribe();
  }

  plansToHarvest(harvestable?: IHarvestableEntity, harvest?: IHarvestableEntityHarvest) {
    return this.character.plannedAction && this.isHarvestingAction(this.character.plannedAction, harvestable, harvest);
  }

  isHarvesting(harvestable?: IHarvestableEntity, harvest?: IHarvestableEntityHarvest) {
    return this.character.ongoingAction && this.isHarvestingAction(this.character.ongoingAction, harvestable, harvest);
  }

  protected formatExpectedHarvest(items: ItemStack[]) {
    return items.map(s => `${s.count}x ${s.item.name}`).join(', ');
  }

  private isHarvestingAction(action: Action, harvestable?: IHarvestableEntity, harvest?: IHarvestableEntityHarvest) {
    if (!(action instanceof HarvestAction) || !this.harvestables.find(h => h.id === (action as HarvestAction).targetId)) {
      return false;
    }

    if (harvestable && action.targetId !== harvestable.id) {
      return false;
    }

    if (harvest && action.harvest.name !== harvest.name) {
      return false;
    }

    return true;
  }
}
