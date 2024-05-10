import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { forkJoin, map, switchMap } from 'rxjs';
import {
  AdminGameContentApiClient,
  AdminGameStateApiClient,
  HarvestableEntityHarvestMinimal,
  Job,
  LocationMinimal,
  StaticObject,
} from '../../../../api/admin-api-client.generated';
import { TeamService } from '../../services/team/team.service';
import { MapComponent, MapMarker } from '../../widgets/map/map.component';

@Component({
  selector: 'app-map-page',
  standalone: true,
  templateUrl: './map-page.component.html',
  imports: [CommonModule, MapComponent],
})
export class MapPageComponent implements OnInit {
  protected locations: LocationMinimal[] = [];
  protected harvestable: StaticObject[] = [];
  protected harvestableMarkers: MapMarker[] = [];
  protected markers: MapMarker[] = [];

  constructor(
    private adminGameContentApiClient: AdminGameContentApiClient,
    private adminGameStateApiClient: AdminGameStateApiClient,
    private teamService: TeamService,
  ) {}

  ngOnInit(): void {
    forkJoin({ locations: this.loadLocations(), harvestables: this.loadHarvestableInstances() })
      .pipe(
        switchMap(() => this.teamService.team$),
        map(team => {
          this.markers = [
            ...(team?.characters.map((c): MapMarker => ({ shape: 'circle', color: 'cyan', positionX: c.location.positionX, positionY: c.location.positionY })) ?? []),
            ...this.harvestableMarkers,
          ];
        }),
      )
      .subscribe();
  }

  private loadLocations() {
    return this.adminGameContentApiClient.searchLocationsMinimal(1, 99999999).pipe(map(locations => (this.locations = locations.items)));
  }

  private loadHarvestableInstances() {
    return this.adminGameContentApiClient.searchJobs(1, 99999999).pipe(
      switchMap(jobs => {
        const work = [];
        for (const job of jobs.items) {
          for (const harvest of job.harvests) {
            for (const target of harvest.targets) {
              work.push(
                this.adminGameStateApiClient.searchStaticObjectInstances(target.id, 1, 99999999).pipe(map(instances => ({ job, harvest, target, instances: instances.items }))),
              );
            }
          }
        }

        return forkJoin(work);
      }),
      map(harvestables => {
        const markers: { [staticObjectId: string]: { [x: number]: { [y: number]: MapMarker } } } = {};
        for (const harvestable of harvestables) {
          if (!markers[harvestable.target.id]) {
            markers[harvestable.target.id] = {};
          }

          for (const instance of harvestable.instances) {
            if (!markers[harvestable.target.id][instance.location.positionX]) {
              markers[harvestable.target.id][instance.location.positionX] = {};
            }

            if (!markers[harvestable.target.id][instance.location.positionX][instance.location.positionY]) {
              markers[harvestable.target.id][instance.location.positionX][instance.location.positionY] = {
                shape: 'circle',
                color: 'green',
                alpha: 0,
                positionX: instance.location.positionX,
                positionY: instance.location.positionY,
              };
            }

            markers[harvestable.target.id][instance.location.positionX][instance.location.positionY].alpha =
              (markers[harvestable.target.id][instance.location.positionX][instance.location.positionY].alpha ?? 0) + 0.1;
          }
        }

        this.harvestableMarkers = Object.values(markers).flatMap(x => Object.values(x).flatMap(x => Object.values(x)));
      }),
    );
  }

  private getColor(job: Job, harvest: HarvestableEntityHarvestMinimal, object: StaticObject) {
    switch (job.name) {
      case 'gatherer':
        return 'green';
      default:
        return 'darkgrey';
    }
  }
}
