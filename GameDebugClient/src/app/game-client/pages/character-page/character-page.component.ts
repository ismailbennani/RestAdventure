import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ReplaySubject, combineLatest, map, switchMap, tap } from 'rxjs';
import { Action, CharactersApiClient, Team, TeamCharacter } from '../../../../api/game-api-client.generated';
import { SpinnerComponent } from '../../../common/spinner/spinner.component';
import { CurrentPageService } from '../../services/current-page.service';
import { GameService } from '../../services/game.service';
import { TeamService } from '../../services/team/team.service';
import { CharacterActionUtils } from '../../utils/character-action-utils';
import { CharacterCombatsComponent } from '../../widgets/character-combats/character-combats.component';
import { CharacterHarvestsComponent } from '../../widgets/character-harvests/character-harvests.component';
import { CharacterHistoryComponent } from '../../widgets/character-history/character-history.component';
import { CharacterMonstersComponent } from '../../widgets/character-monsters/character-monsters.component';
import { CharacterMovementsComponent } from '../../widgets/character-movements/character-movements.component';
import { CharacterComponent } from '../../widgets/character/character.component';
import { CombatHistoryComponent } from '../../widgets/combat-history/combat-history.component';
import { CombatInPreparationComponent } from '../../widgets/combat-in-preparation/combat-in-preparation.component';
import { CombatComponent } from '../../widgets/combat/combat.component';
import { InventoryComponent } from '../../widgets/inventory/inventory.component';
import { JobsComponent } from '../../widgets/jobs/jobs.component';

@Component({
  selector: 'app-character-page',
  templateUrl: './character-page.component.html',
  standalone: true,
  imports: [
    CommonModule,
    SpinnerComponent,
    InventoryComponent,
    CharacterComponent,
    CharacterHistoryComponent,
    JobsComponent,
    CombatComponent,
    CombatInPreparationComponent,
    CharacterMovementsComponent,
    CharacterHarvestsComponent,
    CharacterMonstersComponent,
    CombatHistoryComponent,
    CharacterCombatsComponent,
  ],
})
export class CharacterPageComponent implements OnInit {
  protected loading: boolean = false;
  protected characterId: string | undefined;
  protected character: TeamCharacter | undefined;

  private team: Team | undefined;
  private refreshSubject: ReplaySubject<void> = new ReplaySubject<void>(1);

  constructor(
    private route: ActivatedRoute,
    private currentPageService: CurrentPageService,
    private gameService: GameService,
    private teamService: TeamService,
    private charactersApiClient: CharactersApiClient,
  ) {}

  ngOnInit(): void {
    combineLatest({
      characterId: this.route.paramMap.pipe(map(paramMap => paramMap.get('character-id') ?? undefined)),
      team: this.teamService.team$,
    }).subscribe(({ characterId, team }) => {
      this.characterId = characterId;
      this.team = team;
      this.refreshSubject.next();
    });

    this.loading = true;
    this.refreshSubject
      .pipe(
        tap(() => {
          if (!this.characterId || !this.team) {
            this.loading = true;
            return;
          }

          this.character = this.team?.characters.find(c => c.id === this.characterId);

          if (!this.character && this.team && this.team.characters.length > 0) {
            this.currentPageService.openCharacter(this.team.characters[0]);
            return;
          }

          if (!this.character) {
            this.currentPageService.openHome();
            return;
          }

          this.loading = false;
        }),
      )
      .subscribe();
  }

  actionToString(action: Action) {
    return CharacterActionUtils.toString(action);
  }

  deleteCharacter() {
    if (!this.characterId) {
      throw new Error('Character id not found');
    }

    this.charactersApiClient
      .deleteCharacter(this.characterId)
      .pipe(switchMap(() => this.gameService.refreshNow(true)))
      .subscribe();
  }
}
