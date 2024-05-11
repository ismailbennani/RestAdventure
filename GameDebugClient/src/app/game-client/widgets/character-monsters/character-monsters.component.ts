import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, switchMap, tap } from 'rxjs';
import { Action, IMonsterGroup, JoinPveCombatAction, PveApiClient, StartPveCombatAction, TeamCharacter } from '../../../../api/game-api-client.generated';
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

  protected monsterGroups: (IMonsterGroup & { display: string; totalLevel: number })[] = [];

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
        tap(monsterGroups => {
          this.monsterGroups = monsterGroups.map(g => ({
            ...g,
            display: g.monsters.map(m => `${m.species.name} lv. ${m.level}`).join(', '),
            totalLevel: g.monsters.reduce((level, m) => level + m.level, 0),
          }));
        }),
      )
      .subscribe();
  }

  attack(monsterGroup: IMonsterGroup) {
    if (!this.character || !monsterGroup.canAttackOrJoin) {
      return;
    }

    if (monsterGroup.attacked) {
      this.pveApiClient
        .joinCombat(this.character.id, monsterGroup.id)
        .pipe(switchMap(_ => this.gameService.refreshNow(true)))
        .subscribe();
    } else {
      this.pveApiClient
        .attackMonsters(this.character.id, monsterGroup.id)
        .pipe(switchMap(_ => this.gameService.refreshNow(true)))
        .subscribe();
    }
  }

  plansToAttackOrJoin(monsterGroup?: IMonsterGroup) {
    return this.character?.plannedAction && this.isCombatAction(this.character.plannedAction, monsterGroup);
  }

  isAttacking(monsterGroup: IMonsterGroup) {
    return this.character?.ongoingAction && this.isCombatAction(this.character.ongoingAction, monsterGroup);
  }

  private isCombatAction(action: Action, monsterGroup?: IMonsterGroup) {
    if (!(action instanceof StartPveCombatAction) && !(action instanceof JoinPveCombatAction)) {
      return false;
    }

    if (!monsterGroup) {
      return true;
    }

    return action.monsterGroup.id === monsterGroup.id;
  }
}
