import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, combineLatest, map, switchMap, tap } from 'rxjs';
import {
  ActionEndedHistoryEntry,
  ActionStartedHistoryEntry,
  CharacterCombatEndedHistoryEntry,
  CharacterCombatStartedHistoryEntry,
  CharacterCreatedHistoryEntry,
  CharacterDeletedHistoryEntry,
  CharacterHistoryEntry,
  CharacterInventoryChangedHistoryEntry,
  CharacterJobGainedExperienceHistoryEntry,
  CharacterJobLeveledUpHistoryEntry,
  CharacterLearnedJobHistoryEntry,
  CharacterMoveLocationHistoryEntry,
  CharacterStartedCombatPreparationHistoryEntry,
  CharacterTeleporteLocationHistoryEntry,
  CharactersApiClient,
  CombatEntityInHistoryEntry,
  CombatSide,
  TeamCharacter,
} from '../../../../api/game-api-client.generated';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-character-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './character-history.component.html',
})
export class CharacterHistoryComponent implements OnInit {
  @Input({ required: true })
  public set character(value: TeamCharacter) {
    this.characterSubject.next(value);
  }

  @Input() nEntries: number = 20;

  protected loading: boolean = false;
  protected history: { tick: number; entries: CharacterHistoryEntry[] }[] = [];

  private characterSubject: ReplaySubject<TeamCharacter> = new ReplaySubject<TeamCharacter>(1);

  constructor(
    private gameService: GameService,
    private charactersApiClient: CharactersApiClient,
  ) {}

  ngOnInit(): void {
    this.loading = true;
    combineLatest({
      state: this.gameService.state$,
      character: this.characterSubject,
    })
      .pipe(
        switchMap(({ character }) => this.charactersApiClient.searchCharacterHistory(character.id, 1, this.nEntries)),
        map(history => {
          const historyObj: { [tick: number]: CharacterHistoryEntry[] } = {};
          for (const entry of history.items) {
            if (!historyObj[entry.tick]) {
              historyObj[entry.tick] = [];
            }

            historyObj[entry.tick].push(entry);
          }

          const ticks: number[] = Object.keys(historyObj).map(t => Number(t));
          ticks.sort((a, b) => b - a);

          this.history = [];
          for (const tick of ticks) {
            this.history.push({ tick, entries: historyObj[tick] });
          }
        }),
        tap(() => (this.loading = false)),
      )
      .subscribe();
  }

  protected getMessage(entry: CharacterHistoryEntry) {
    if (entry instanceof CharacterCreatedHistoryEntry) {
      return `Character created`;
    }

    if (entry instanceof CharacterDeletedHistoryEntry) {
      return `Character deleted`;
    }

    if (entry instanceof CharacterMoveLocationHistoryEntry) {
      if (entry.oldLocationId) {
        if (entry.oldLocationAreaId !== entry.newLocationAreaId) {
          return `Moved from [${entry.oldLocationPositionX}, ${entry.oldLocationPositionY}] (${entry.oldLocationAreaName}) to [${entry.newLocationPositionX}, ${entry.newLocationPositionY}] (${entry.newLocationAreaName})`;
        } else {
          return `Moved from [${entry.oldLocationPositionX}, ${entry.oldLocationPositionY}] to [${entry.newLocationPositionX}, ${entry.newLocationPositionY}] (${entry.newLocationAreaName})`;
        }
      } else {
        return `Moved to [${entry.newLocationPositionX}, ${entry.newLocationPositionY}] (${entry.newLocationAreaName})`;
      }
    }

    if (entry instanceof CharacterTeleporteLocationHistoryEntry) {
      if (entry.oldLocationId) {
        if (entry.oldLocationAreaId !== entry.newLocationAreaId) {
          return `Teleported from [${entry.oldLocationPositionX}, ${entry.oldLocationPositionY}] (${entry.oldLocationAreaName}) to [${entry.newLocationPositionX}, ${entry.newLocationPositionY}] (${entry.newLocationAreaName})`;
        } else {
          return `Teleported from [${entry.oldLocationPositionX}, ${entry.oldLocationPositionY}] to [${entry.newLocationPositionX}, ${entry.newLocationPositionY}] (${entry.newLocationAreaName})`;
        }
      } else {
        return `Teleported to [${entry.newLocationPositionX}, ${entry.newLocationPositionY}] (${entry.newLocationAreaName})`;
      }
    }

    if (entry instanceof CharacterInventoryChangedHistoryEntry) {
      if (entry.oldCount < entry.newCount) {
        return `Picked ${entry.newCount - entry.oldCount}x ${entry.itemName}`;
      } else {
        return `Dropped ${entry.oldCount - entry.newCount}x ${entry.itemName}`;
      }
    }

    if (entry instanceof ActionStartedHistoryEntry) {
      return `Started action ${entry.actionName}`;
    }

    if (entry instanceof ActionEndedHistoryEntry) {
      return `Ended action ${entry.actionName}`;
    }

    if (entry instanceof CharacterLearnedJobHistoryEntry) {
      return `Learned job ${entry.jobName}`;
    }

    if (entry instanceof CharacterJobGainedExperienceHistoryEntry) {
      return `${entry.jobName}: +${entry.newExperience - entry.oldExperience} exp (${entry.newExperience})`;
    }

    if (entry instanceof CharacterJobLeveledUpHistoryEntry) {
      return `${entry.jobName}: +${entry.newLevel - entry.oldLevel} lv. (${entry.newLevel})`;
    }

    if (entry instanceof CharacterStartedCombatPreparationHistoryEntry) {
      return `Started combat preparation: ${entry.attackers.map(e => e.name).join(',')} v. ${entry.defenders.map(e => e.name).join(',')} at ${entry.locationAreaName} [${entry.locationPositionX}, ${entry.locationPositionY}]`;
    }

    if (entry instanceof CharacterCombatStartedHistoryEntry) {
      return `Started combat: ${entry.attackers.map(e => e.name).join(',')} v. ${entry.defenders.map(e => e.name).join(',')} at ${entry.locationAreaName} [${entry.locationPositionX}, ${entry.locationPositionY}]`;
    }

    if (entry instanceof CharacterCombatEndedHistoryEntry) {
      let winners: CombatEntityInHistoryEntry[] = [];
      switch (entry.winner) {
        case CombatSide.Attackers:
          winners = entry.attackers;
          break;
        case CombatSide.Defenders:
          winners = entry.defenders;
          break;
      }

      return `Combat ended in ${entry.duration} turns, winner: ${winners.map(e => e.name).join(', ')}`;
    }

    return '???';
  }
}
