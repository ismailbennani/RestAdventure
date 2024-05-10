import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, switchMap, tap } from 'rxjs';
import { Action, HarvestAction, IHarvestableEntity, IHarvestableEntityHarvest, ItemStack, JobsHarvestApiClient, TeamCharacter } from '../../../../api/game-api-client.generated';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-character-harvests',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './character-harvests.component.html',
})
export class CharacterHarvestsComponent implements OnInit {
  @Input({ required: true })
  public get character(): TeamCharacter {
    return this._character;
  }
  public set character(value: TeamCharacter) {
    this._character = value;
    this.characterSubject.next(value);
  }

  protected harvestables: (IHarvestableEntity & { count: number })[] = [];

  private _character: TeamCharacter = null!;
  private characterSubject: ReplaySubject<TeamCharacter> = new ReplaySubject<TeamCharacter>(1);

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
    if (!(action instanceof HarvestAction) || !this.harvestables.find(h => h.id === (action as HarvestAction).target.id)) {
      return false;
    }

    if (harvestable && action.target.id !== harvestable.id) {
      return false;
    }

    if (harvest && action.harvest.name !== harvest.name) {
      return false;
    }

    return true;
  }
}
