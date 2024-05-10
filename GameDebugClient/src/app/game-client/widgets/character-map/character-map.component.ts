import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ReplaySubject, combineLatest, map } from 'rxjs';
import { AdminGameContentApiClient, Location } from '../../../../api/admin-api-client.generated';
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
  protected markers: MapMarker[] = [];

  private characterSubject: ReplaySubject<TeamCharacter> = new ReplaySubject<TeamCharacter>(1);

  constructor(
    private adminGameContentApiClient: AdminGameContentApiClient,
    private teamService: TeamService,
  ) {}

  ngOnInit(): void {
    this.adminGameContentApiClient
      .searchLocations(1, 99999999)
      .pipe(map(locations => (this.locations = locations.items)))
      .subscribe();

    combineLatest({
      character: this.characterSubject,
      team: this.teamService.team$,
    }).subscribe(({ character, team }) => {
      this.centerAt = [character.location.positionX, character.location.positionY];
      this.currentCharacterId = character.id;
      this.markers = [
        { shape: 'circle', color: 'blue', positionX: character.location.positionX, positionY: character.location.positionY },
        ...(team?.characters
          .filter(c => c.id !== character.id)
          .map((c): MapMarker => ({ shape: 'circle', color: 'cyan', positionX: c.location.positionX, positionY: c.location.positionY })) ?? []),
      ];
    });
  }
}
