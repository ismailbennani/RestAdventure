import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { map } from 'rxjs';
import { AdminGameContentApiClient } from '../../../../api/admin-api-client.generated';
import { Location } from '../../../../api/game-api-client.generated';
import { TeamService } from '../../services/team/team.service';
import { MapComponent, MapMarker } from '../../widgets/map/map.component';

@Component({
  selector: 'app-map-page',
  standalone: true,
  templateUrl: './map-page.component.html',
  imports: [CommonModule, MapComponent],
})
export class MapPageComponent implements OnInit {
  protected locations: Location[] = [];
  protected markers: MapMarker[] = [];

  constructor(
    private adminGameContentApiClient: AdminGameContentApiClient,
    private teamService: TeamService,
  ) {}

  ngOnInit(): void {
    this.adminGameContentApiClient
      .searchLocations(1, 1000)
      .pipe(map(locations => (this.locations = locations.items)))
      .subscribe();

    this.teamService.team$.subscribe(team => {
      this.markers = team?.characters.map((c): MapMarker => ({ shape: 'circle', color: 'cyan', positionX: c.location.positionX, positionY: c.location.positionY })) ?? [];
    });
  }
}
