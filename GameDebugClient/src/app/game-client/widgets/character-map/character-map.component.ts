import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, map } from 'rxjs';
import { AdminGameContentApiClient, Location } from '../../../../api/admin-api-client.generated';
import { TeamCharacter } from '../../../../api/game-api-client.generated';
import { MapComponent } from '../map/map.component';

@Component({
  selector: 'app-character-map',
  standalone: true,
  templateUrl: './character-map.component.html',
  imports: [CommonModule, MapComponent],
})
export class CharacterMapComponent implements OnInit {
  @Input({ required: true })
  public get character(): TeamCharacter {
    return this._character;
  }
  public set character(value: TeamCharacter) {
    this._character = value;
    this.characterSubject.next(value);
  }
  private _character: TeamCharacter = null!;

  protected currentCharacterId: string | undefined;
  protected locations: Location[] = [];
  protected centerAt: [number, number] | undefined;

  private characterSubject: ReplaySubject<TeamCharacter> = new ReplaySubject<TeamCharacter>(1);

  constructor(private adminGameContentApiClient: AdminGameContentApiClient) {}

  ngOnInit(): void {
    this.adminGameContentApiClient
      .searchLocations(1, 1000)
      .pipe(map(locations => (this.locations = locations.items)))
      .subscribe();

    this.characterSubject.subscribe(character => {
      if (this.currentCharacterId == character.id && this.centerAt && character.location.positionX == this.centerAt[0] && character.location.positionY == this.centerAt[1]) {
        return;
      }

      this.centerAt = [character.location.positionX, character.location.positionY];
      this.currentCharacterId = character.id;
    });
  }
}
