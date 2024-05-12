import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, numberAttribute } from '@angular/core';
import { ReplaySubject, catchError, combineLatest, debounceTime, map, of, switchMap, tap } from 'rxjs';
import {
  ArchivedCombat,
  Character,
  CombatEndedHistoryEntry,
  CombatEntityAttackedHistoryEntry,
  CombatEntityDiedHistoryEntry,
  CombatEntityJoinedHistoryEntry,
  CombatEntityLeftHistoryEntry,
  CombatHistoryEntry,
  CombatInPreparation,
  CombatPreparationStartedHistoryEntry,
  CombatStartedHistoryEntry,
  CombatsHistoryApiClient,
  OngoingCombat,
  SearchResultOfCombatHistoryEntry,
} from '../../../../api/game-api-client.generated';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-combat-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './combat-history.component.html',
})
export class CombatHistoryComponent implements OnInit {
  @Input({ required: true })
  public set character(value: Character) {
    this.characterSubject.next(value);
  }

  @Input({ required: true })
  public set combat(value: CombatInPreparation | OngoingCombat | ArchivedCombat) {
    this.combatSubject.next(value);
  }

  @Input({ transform: numberAttribute }) nEntries: number = 20;

  protected loading: boolean = false;
  protected history: { turn: number; preparation: boolean; entries: CombatHistoryEntry[] }[] = [];

  private characterSubject: ReplaySubject<Character> = new ReplaySubject<Character>(1);
  private combatSubject: ReplaySubject<CombatInPreparation | OngoingCombat | ArchivedCombat> = new ReplaySubject<CombatInPreparation | OngoingCombat | ArchivedCombat>(1);

  constructor(
    private gameService: GameService,
    private combatsHistoryApiClient: CombatsHistoryApiClient,
  ) {}

  ngOnInit(): void {
    this.loading = true;
    combineLatest({
      state: this.gameService.state$,
      character: this.characterSubject,
      combat: this.combatSubject,
    })
      .pipe(
        debounceTime(10),
        switchMap(({ character, combat }) => {
          if (combat instanceof ArchivedCombat) {
            return this.combatsHistoryApiClient.searchArchivedCombatHistory(character.id, combat.id, 1, this.nEntries).pipe(
              catchError(e => {
                console.error(e);
                return of(new SearchResultOfCombatHistoryEntry({ items: [], pageNumber: 1, pageSize: this.nEntries, totalItemsCount: 0, totalPagesCount: 0 }));
              }),
            );
          } else {
            return this.combatsHistoryApiClient.searchCombatHistory(character.id, combat.id, 1, this.nEntries).pipe(
              catchError(e => {
                console.error(e);
                return of(new SearchResultOfCombatHistoryEntry({ items: [], pageNumber: 1, pageSize: this.nEntries, totalItemsCount: 0, totalPagesCount: 0 }));
              }),
            );
          }
        }),
        map(history => {
          const historyObj: { [turn: number]: CombatHistoryEntry[] } = {};
          for (const entry of history.items) {
            if (!historyObj[entry.turn]) {
              historyObj[entry.turn] = [];
            }

            historyObj[entry.turn].push(entry);
          }

          const turns: number[] = Object.keys(historyObj).map(t => Number(t));
          turns.sort((a, b) => b - a);

          this.history = [];
          for (const turn of turns) {
            const notPreparationHistoryObjs = historyObj[turn].filter(h => !this.isPreparationHistory(h));
            if (notPreparationHistoryObjs.length == 0) {
              continue;
            }

            this.history.push({ turn, preparation: false, entries: notPreparationHistoryObjs });
          }

          for (const turn of turns) {
            const preparationHistoryObjs = historyObj[turn].filter(h => this.isPreparationHistory(h));
            if (preparationHistoryObjs.length == 0) {
              continue;
            }

            this.history.push({ turn, preparation: true, entries: preparationHistoryObjs });
          }
        }),
        tap(() => (this.loading = false)),
      )
      .subscribe();
  }

  protected getMessage(entry: CombatHistoryEntry) {
    if (entry instanceof CombatPreparationStartedHistoryEntry) {
      return 'Preparation started';
    }

    if (entry instanceof CombatStartedHistoryEntry) {
      return 'Combat started';
    }

    if (entry instanceof CombatEntityJoinedHistoryEntry) {
      return `${entry.entityName} joined the ${entry.side}`;
    }

    if (entry instanceof CombatEntityLeftHistoryEntry) {
      return `${entry.entityName} left the ${entry.side}`;
    }

    if (entry instanceof CombatEntityAttackedHistoryEntry) {
      return `${entry.attackerName} attacked ${entry.targetName}: -${entry.damage} HP`;
    }

    if (entry instanceof CombatEntityDiedHistoryEntry) {
      return `${entry.entityName} killed by ${entry.attackerName}`;
    }

    if (entry instanceof CombatEndedHistoryEntry) {
      return `End, ${entry.winner} won`;
    }

    return '???';
  }

  private isPreparationHistory(entry: CombatHistoryEntry) {
    return entry instanceof CombatPreparationStartedHistoryEntry || entry instanceof CombatEntityJoinedHistoryEntry || entry instanceof CombatEntityLeftHistoryEntry;
  }
}
