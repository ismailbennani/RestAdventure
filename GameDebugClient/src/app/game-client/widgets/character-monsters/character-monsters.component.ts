import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, switchMap, tap } from 'rxjs';
import { Action, JoinPveCombatAction, MonsterGroup, PveApiClient, PveCombatAction, StartPveCombatAction, TeamCharacter } from '../../../../api/game-api-client.generated';
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

  attack(monsterGroup: MonsterGroup) {
    if (!this.character || !monsterGroup.canAttack) {
      return;
    }

    this.pveApiClient
      .attackMonsters(this.character.id, monsterGroup.id)
      .pipe(switchMap(_ => this.gameService.refreshNow(true)))
      .subscribe();
  }

  plansToAttack(monsterGroup?: MonsterGroup) {
    return this.character?.plannedAction && this.isCombatAction(this.character.plannedAction, monsterGroup);
  }

  isAttacking(monsterGroup: MonsterGroup) {
    return this.character?.ongoingAction && this.isCombatAction(this.character.ongoingAction, monsterGroup);
  }

  private isCombatAction(action: Action, monsterGroup?: MonsterGroup) {
    if (!(action instanceof PveCombatAction) && !(action instanceof StartPveCombatAction) && !(action instanceof JoinPveCombatAction)) {
      return false;
    }

    if (!monsterGroup) {
      return true;
    }

    const inCombat = [...action.attackers.map(a => a.id), ...action.defenders.map(a => a.id)];
    const inGroup = monsterGroup.monsters.map(m => m.id);

    return inGroup.some(id => inCombat.includes(id));
  }
}
