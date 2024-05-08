import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, switchMap, tap } from 'rxjs';
import { CharacterInteractWithEntityAction, MonsterGroup, PveApiClient, TeamCharacter } from '../../../../api/game-api-client.generated';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-character-monsters',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './character-monsters.component.html',
})
export class CharacterMonstersComponent implements OnInit {
  @Input({ required: true })
  public get character(): TeamCharacter {
    return this._character;
  }
  public set character(value: TeamCharacter) {
    this._character = value;
    this.characterSubject.next(value);
  }

  protected monsterGroups: MonsterGroup[] = [];

  private _character: TeamCharacter = null!;
  private characterSubject: ReplaySubject<TeamCharacter> = new ReplaySubject<TeamCharacter>(1);

  constructor(
    private gameService: GameService,
    private pveApiClient: PveApiClient,
  ) {}

  ngOnInit(): void {
    this.characterSubject
      .pipe(
        switchMap(character => this.pveApiClient.getMonsters(character.id)),
        tap(monsterGroups => (this.monsterGroups = monsterGroups)),
      )
      .subscribe();
  }

  plansToAttack(monsterGroup?: MonsterGroup) {
    if (!this.character?.plannedAction || !(this.character.plannedAction instanceof CharacterInteractWithEntityAction)) {
      return false;
    }

    if (monsterGroup && this.character.plannedAction.interaction.name == 'combat' && this.character.plannedAction.target.id != monsterGroup.id) {
      return false;
    }

    return true;
  }

  attack(monsterGroup: MonsterGroup) {
    if (!this.character || !monsterGroup.canAttack) {
      return;
    }

    this.pveApiClient
      .attackMonsters(this.character.id, monsterGroup.id)
      .pipe(switchMap(_ => this.gameService.refreshNow(true)))
      .subscribe();
  }

  isAttacking(monsterGroup: MonsterGroup) {
    if (!this.character?.currentInteraction) {
      return false;
    }

    if (monsterGroup && this.character.currentInteraction.interaction.name == 'combat' && this.character.currentInteraction.target.id != monsterGroup.id) {
      return false;
    }

    return true;
  }
}
