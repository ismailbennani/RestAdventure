import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbDropdownModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { Observable, catchError, finalize, map, of, switchMap, tap } from 'rxjs';
import { AdminGameApiClient, Player } from '../../../api/admin-api-client.generated';
import {
  CharacterAction,
  CharacterClass,
  CharacterMoveToLocationAction,
  CreateCharacterRequest,
  GameApiClient,
  GameSettings,
  GameState,
  IMapLocationMinimal,
  Team,
  TeamApiClient,
  TeamCharacter,
  TeamCharactersActionsApiClient,
  TeamCharactersApiClient,
} from '../../../api/game-api-client.generated';
import { SpinnerComponent } from '../../common/spinner/spinner.component';
import { SELECT_PLAYER_ROUTE } from '../../routes';
import { SelectedPlayerService } from '../select-player/selected-player.service';

@Component({
  selector: 'app-game-client',
  standalone: true,
  templateUrl: './game-client.component.html',
  imports: [CommonModule, NgbDropdownModule, NgbTooltipModule, SpinnerComponent],
})
export class GameClientComponent implements OnInit {
  protected settings: GameSettings = new GameSettings();

  protected loading: boolean = false;
  protected player: Player;
  protected gameState: GameState | undefined;
  protected characters: (TeamCharacter | 'loading' | undefined)[] = [];
  protected performingAction: { [characterId: string]: boolean } = {};

  protected characterClasses: { value: CharacterClass; display: string }[] = [
    {
      value: CharacterClass.Knight,
      display: 'Knight',
    },
    {
      value: CharacterClass.Mage,
      display: 'Mage',
    },
    {
      value: CharacterClass.Scout,
      display: 'Scout',
    },
    {
      value: CharacterClass.Dealer,
      display: 'Dealer',
    },
  ];

  constructor(
    selectedPlayerService: SelectedPlayerService,
    private adminGameApiClient: AdminGameApiClient,
    private gameApiClient: GameApiClient,
    private teamApiClient: TeamApiClient,
    private teamCharactersApiClient: TeamCharactersApiClient,
    private teamCharactersActionsApiClient: TeamCharactersActionsApiClient,
    private router: Router,
  ) {
    this.player = selectedPlayerService.get();
  }

  ngOnInit(): void {
    this.loading = true;

    this.gameApiClient
      .getGameSettings()
      .pipe(
        switchMap(settings => {
          this.settings = settings;
          return this.refreshCharacters();
        }),
        finalize(() => (this.loading = false)),
      )
      .subscribe();

    this.refreshGameState();
  }

  play() {
    this.adminGameApiClient
      .startSimulation()
      .pipe(map(_ => this.refreshGameState()))
      .subscribe();
  }

  pause() {
    this.adminGameApiClient
      .stopSimulation()
      .pipe(map(_ => this.refreshGameState()))
      .subscribe();
  }

  step() {
    this.adminGameApiClient
      .tickNow()
      .pipe(map(_ => this.refreshGameState()))
      .subscribe();
  }

  createCharacter(name: string, cls: string) {
    const index = this.characters.findIndex(c => !c);
    if (index < 0) {
      throw new Error('Max team size reached already');
    }

    this.characters[index] = 'loading';
    this.teamCharactersApiClient
      .createCharacter(new CreateCharacterRequest({ name, class: cls as CharacterClass }))
      .pipe(
        map(character => (this.characters[index] = character)),
        catchError(e => {
          console.error('Error while creating character.', e);
          this.characters[index] = undefined;
          return of(void 0);
        }),
      )
      .subscribe();
  }

  moveToLocation(character: TeamCharacter, location: IMapLocationMinimal) {
    this.performingAction[character.id] = true;
    this.teamCharactersActionsApiClient
      .moveToLocation(character.id, location.id)
      .pipe(
        switchMap(_ => this.refreshCharacters()),
        finalize(() => (this.performingAction[character.id] = false)),
      )
      .subscribe();
  }

  characterWillMoveToLocation(character: TeamCharacter, location: IMapLocationMinimal) {
    if (!character.nextAction) {
      return false;
    }

    if (!(character.nextAction instanceof CharacterMoveToLocationAction)) {
      return false;
    }

    return character.nextAction.locationId == location.id;
  }

  changePlayer() {
    this.router.navigateByUrl(SELECT_PLAYER_ROUTE);
  }

  protected actionToString(action: CharacterAction) {
    if (action instanceof CharacterMoveToLocationAction) {
      return `move to ${action.locationId}`;
    }

    return '???';
  }

  private refreshCharacters(): Observable<Team> {
    return this.teamApiClient.getTeam().pipe(
      map(team => {
        this.characters = new Array(this.settings.maxTeamSize);
        for (let i = 0; i < team.characters.length; i++) {
          this.characters[i] = team.characters[i];
        }

        return team;
      }),
    );
  }

  private refreshGameState() {
    this.gameApiClient
      .getGameState()
      .pipe(
        tap(state => {
          if (state.tick != this.gameState?.tick) {
            this.refreshCharacters().subscribe();
          }
        }),
      )
      .subscribe(state => {
        this.gameState = state;
        const now = Date.now();
        const nextTick = state.nextTickDate.getTime();

        const toWait = nextTick > now ? nextTick - now : 1000;

        setTimeout(() => this.refreshGameState(), toWait);
      });
  }
}
