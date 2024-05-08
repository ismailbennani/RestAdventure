import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, combineLatest, map, switchMap, tap } from 'rxjs';
import {
  CharacterAttackedHistoryEntry,
  CharacterCombatEndedHistoryEntry,
  CharacterCombatInPreparationCanceledHistoryEntry,
  CharacterCombatStartedHistoryEntry,
  CharacterCreatedHistoryEntry,
  CharacterDeletedHistoryEntry,
  CharacterEndedInteractionHistoryEntry,
  CharacterHistoryEntry,
  CharacterInventoryChangedHistoryEntry,
  CharacterJobGainedExperienceHistoryEntry,
  CharacterJobLeveledUpHistoryEntry,
  CharacterLearnedJobHistoryEntry,
  CharacterMoveLocationHistoryEntry,
  CharacterPerformedActionHistoryEntry,
  CharacterReceivedAttackHistoryEntry,
  CharacterStartedCombatPreparationHistoryEntry,
  CharacterStartedInteractionHistoryEntry,
  CombatEntityInHistoryEntry,
  CombatSide,
  TeamCharacter,
  TeamCharactersApiClient,
} from '../../../../api/game-api-client.generated';
import { GameService } from '../../services/game.service';
import { CharacterActionUtils } from '../../utils/character-action-utils';

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
    private charactersApiClient: TeamCharactersApiClient,
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

    if (entry instanceof CharacterInventoryChangedHistoryEntry) {
      if (entry.oldCount < entry.newCount) {
        return `Picked ${entry.newCount - entry.oldCount}x ${entry.itemName}`;
      } else {
        return `Dropped ${entry.oldCount - entry.newCount}x ${entry.itemName}`;
      }
    }

    if (entry instanceof CharacterPerformedActionHistoryEntry) {
      if (entry.success) {
        return `Performed action successfully: ${CharacterActionUtils.toString(entry.action)}`;
      } else {
        return `Failed to perform action: ${CharacterActionUtils.toString(entry.action)}. Reason: ${entry.failureReason}`;
      }
    }

    if (entry instanceof CharacterStartedInteractionHistoryEntry) {
      return `Started interaction ${entry.interactionName} with ${entry.targetName}`;
    }

    if (entry instanceof CharacterEndedInteractionHistoryEntry) {
      return `Ended interaction ${entry.interactionName} with ${entry.targetName}`;
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
      return `Started combat preparation: ${entry.team1.map(e => e.name).join(',')} v. ${entry.team2.map(e => e.name).join(',')} at ${entry.locationAreaName} [${entry.locationPositionX}, ${entry.locationPositionY}]`;
    }

    if (entry instanceof CharacterCombatInPreparationCanceledHistoryEntry) {
      return `Combat preparation canceled`;
    }

    if (entry instanceof CharacterCombatStartedHistoryEntry) {
      return `Started combat: ${entry.team1.map(e => e.name).join(',')} v. ${entry.team2.map(e => e.name).join(',')} at ${entry.locationAreaName} [${entry.locationPositionX}, ${entry.locationPositionY}]`;
    }

    if (entry instanceof CharacterAttackedHistoryEntry) {
      const reduction = entry.attackReceived.damage - entry.attackDealt.damage;
      if (reduction === 0) {
        return `Attacked ${entry.targetName}: -${entry.attackReceived.damage} HP`;
      } else {
        return `Attacked ${entry.targetName}: -${entry.attackReceived.damage} HP (${entry.attackDealt.damage}-${reduction})`;
      }
    }

    if (entry instanceof CharacterReceivedAttackHistoryEntry) {
      const reduction = entry.attackReceived.damage - entry.attackDealt.damage;
      if (reduction === 0) {
        return `Attacked by ${entry.attackerName}: -${entry.attackReceived.damage} HP`;
      } else {
        return `Attacked by ${entry.attackerName}: -${entry.attackReceived.damage} HP (${entry.attackDealt.damage}-${reduction})`;
      }
    }

    if (entry instanceof CharacterCombatEndedHistoryEntry) {
      let winners: CombatEntityInHistoryEntry[] = [];
      switch (entry.winner) {
        case CombatSide.Team1:
          winners = entry.team1;
          break;
        case CombatSide.Team2:
          winners = entry.team2;
          break;
      }

      return `Combat ended in ${entry.duration} turns, winner: ${winners.map(e => e.name).join(', ')}`;
    }

    return '???';
  }
}
