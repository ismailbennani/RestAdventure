import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, switchMap, tap } from 'rxjs';
import { CharacterInteractWithEntityAction, HarvestableEntity, HarvestableEntityHarvest, JobsHarvestApiClient, TeamCharacter } from '../../../../api/game-api-client.generated';
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
    if (!this.character) {
      return;
    }

    this.jobsHarvestApiClient
      .harvest(this.character.id, entity.id, harvest.name)
      .pipe(switchMap(_ => this.gameService.refreshNow(true)))
      .subscribe();
  }

  plansToHarvest(harvestable?: HarvestableEntity, harvest?: HarvestableEntityHarvest) {
    if (
      !this.character?.plannedAction ||
      !(this.character.plannedAction instanceof CharacterInteractWithEntityAction) ||
      !this.harvestables.find(h => h.id === (this.character.plannedAction as CharacterInteractWithEntityAction).target.id)
    ) {
      return false;
    }

    if (harvestable && this.character.plannedAction.target.id != harvestable.id) {
      return false;
    }

    if (harvest && this.character.plannedAction.interaction.name != harvest.job.name + '-' + harvest.name) {
      return false;
    }

    return true;
  }

  isHarvesting(harvestable: HarvestableEntity, harvest: HarvestableEntityHarvest) {
    if (!this.character?.currentInteraction) {
      return false;
    }

    return this.character.currentInteraction.target.id == harvestable.id && this.character.currentInteraction.interaction.name == harvest.job.name + '-' + harvest.name;
  }
}
