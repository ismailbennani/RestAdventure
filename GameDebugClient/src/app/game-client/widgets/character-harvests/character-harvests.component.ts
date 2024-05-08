import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, switchMap, tap } from 'rxjs';
import { Action, HarvestAction, HarvestableEntity, HarvestableEntityHarvest, ItemStack, JobsHarvestApiClient, TeamCharacter } from '../../../../api/game-api-client.generated';
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

  protected harvestables: HarvestableEntity[] = [];

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
        tap(harvestables => (this.harvestables = harvestables)),
      )
      .subscribe();
  }

  harvest(entity: HarvestableEntity, harvest: HarvestableEntityHarvest) {
    if (!this.character || !harvest.canHarvest) {
      return;
    }

    this.jobsHarvestApiClient
      .harvest(this.character.id, entity.id, harvest.name)
      .pipe(switchMap(_ => this.gameService.refreshNow(true)))
      .subscribe();
  }

  plansToHarvest(harvestable?: HarvestableEntity, harvest?: HarvestableEntityHarvest) {
    return this.character.plannedAction && this.isHarvestingAction(this.character.plannedAction, harvestable, harvest);
  }

  isHarvesting(harvestable?: HarvestableEntity, harvest?: HarvestableEntityHarvest) {
    return this.character.ongoingAction && this.isHarvestingAction(this.character.ongoingAction, harvestable, harvest);
  }

  protected formatExpectedHarvest(items: ItemStack[]) {
    return items.map(s => `${s.count}x ${s.item.name}`).join(', ');
  }

  private isHarvestingAction(action: Action, harvestable?: HarvestableEntity, harvest?: HarvestableEntityHarvest) {
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
