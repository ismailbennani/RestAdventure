import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ReplaySubject, combineLatest, map } from 'rxjs';
import { AdminGameContentApiClient, LocationMinimal } from '../../../../api/admin-api-client.generated';
import { TeamCharacter } from '../../../../api/game-api-client.generated';
import { TeamService } from '../../services/team/team.service';
import { MapComponent, MapMarker } from '../map/map.component';

@Component({
  selector: 'app-character-map',
  standalone: true,
  templateUrl: './character-map.component.html',
  imports: [CommonModule, MapComponent],
})
export class CharacterMapComponent implements OnInit {
  @ViewChild('map')
  public set map(value: MapComponent | undefined) {
    value?.setZoom(0.8);
  }

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
  protected locations: LocationMinimal[] = [];
  protected centerAt: [number, number] | undefined;
  protected markerGroups: { [characterId: string]: MapMarker[] } = {};

  private characterSubject: ReplaySubject<TeamCharacter> = new ReplaySubject<TeamCharacter>(1);

  constructor(
    private adminGameContentApiClient: AdminGameContentApiClient,
    private teamService: TeamService,
  ) {}

  ngOnInit(): void {
    this.adminGameContentApiClient
      .searchLocationsMinimal(1, 99999999)
      .pipe(map(locations => (this.locations = locations.items)))
      .subscribe();

    combineLatest({
      character: this.characterSubject,
      team: this.teamService.team$,
    }).subscribe(({ character, team }) => {
      this.centerAt = [character.location.positionX, character.location.positionY];
      this.currentCharacterId = character.id;
      this.markerGroups = {};
      this.markerGroups[character.id] = [{ shape: 'circle', color: 'blue', positionX: character.location.positionX, positionY: character.location.positionY }];

      if (team) {
        for (const c of team.characters.filter(c => c.id !== character.id)) {
          this.markerGroups[c.id] = [{ shape: 'circle', color: 'cyan', positionX: c.location.positionX, positionY: c.location.positionY }];
        }
      }
    });
  }
}
