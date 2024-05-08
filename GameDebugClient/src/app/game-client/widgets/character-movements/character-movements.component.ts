import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, switchMap, tap } from 'rxjs';
import { LocationMinimal } from '../../../../api/admin-api-client.generated';
import { LocationWithAccess, LocationsApiClient, MoveAction, TeamCharacter } from '../../../../api/game-api-client.generated';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-character-movements',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './character-movements.component.html',
})
export class CharacterMovementsComponent implements OnInit {
  @Input({ required: true })
  public get character(): TeamCharacter {
    return this._character;
  }
  public set character(value: TeamCharacter) {
    this._character = value;
    this.characterSubject.next(value);
  }

  protected locations: LocationWithAccess[] = [];

  private _character: TeamCharacter = null!;
  private characterSubject: ReplaySubject<TeamCharacter> = new ReplaySubject<TeamCharacter>(1);

  constructor(
    private gameService: GameService,
    private locationsApiClient: LocationsApiClient,
  ) {}

  ngOnInit(): void {
    this.characterSubject
      .pipe(
        switchMap(character => this.locationsApiClient.getAccessibleLocations(character.id)),
        tap(locations => (this.locations = locations)),
      )
      .subscribe();
  }

  moveToLocation(location: LocationWithAccess) {
    if (!this.character || !location.isAccessible) {
      return;
    }

    this.locationsApiClient
      .moveToLocation(this.character.id, location.location.id)
      .pipe(switchMap(_ => this.gameService.refreshNow(true)))
      .subscribe();
  }

  plansToMove(location?: LocationMinimal) {
    if (!this.character?.plannedAction || !(this.character.plannedAction instanceof MoveAction)) {
      return false;
    }

    if (location && this.character.plannedAction.location.id != location.id) {
      return false;
    }

    return true;
  }
}
