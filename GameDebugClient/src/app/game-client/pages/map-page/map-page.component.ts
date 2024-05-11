import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { forkJoin, map, switchMap } from 'rxjs';
import {
  AdminGameContentApiClient,
  AdminGameStateApiClient,
  Entity,
  HarvestableEntityHarvestMinimal,
  Job,
  LocationMinimal,
  StaticObject,
} from '../../../../api/admin-api-client.generated';
import { Team } from '../../../../api/game-api-client.generated';
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
  protected markers: MapMarker[] = [];
  protected markerDescriptions: { category: string; markers: { marker: string; display: string; color: string }[] }[] = [];
  protected markerConfiguration: { [name: string]: boolean } = {};

  private team: Team | undefined;
  private harvestables: { job: Job; harvest: HarvestableEntityHarvestMinimal; target: StaticObject; instances: Entity[] }[] = [];

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
          this.team = team;
          this.refreshMarkerDescriptions();
          this.refreshMarkers();
        }),
      )
      .subscribe();
  }

  protected setEnabled(marker: string, enabled: boolean) {
    this.markerConfiguration[marker] = enabled;
    this.refreshMarkers();
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
        this.harvestables = harvestables;
        this.refreshMarkerDescriptions();
        this.refreshMarkers();
      }),
    );
  }

  private getColor(job: Job, harvest: HarvestableEntityHarvestMinimal, object: StaticObject) {
    switch (job.name) {
      case 'gatherer':
        switch (object.name) {
          case 'Apple Tree':
            return 'crimson';
          case 'Pear Tree':
            return 'orange';
        }

        return 'green';
      default:
        return 'darkgrey';
    }
  }

  private refreshMarkers() {
    this.markers = [];

    if (this.team) {
      const description = this.markerDescriptions.find(d => d.category == 'Team');
      for (const character of this.team.characters) {
        if (!this.markerConfiguration[character.id]) {
          continue;
        }

        this.markers.push({
          shape: 'circle',
          color: description?.markers.find(c => c.marker === character.id)?.color ?? 'cyan',
          positionX: character.location.positionX,
          positionY: character.location.positionY,
        });
      }
    }

    const markers: { [staticObjectId: string]: { [x: number]: { [y: number]: MapMarker } } } = {};
    for (const harvestable of this.harvestables) {
      if (!this.markerConfiguration[harvestable.target.id]) {
        continue;
      }

      const description = this.markerDescriptions.find(c => c.category === harvestable.job.name)?.markers.find(m => m.marker === harvestable.target.id);

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
            color: description?.color ?? 'grey',
            alpha: 0,
            positionX: instance.location.positionX,
            positionY: instance.location.positionY,
          };
        }

        markers[harvestable.target.id][instance.location.positionX][instance.location.positionY].alpha =
          (markers[harvestable.target.id][instance.location.positionX][instance.location.positionY].alpha ?? 0) + 0.2;
      }
    }

    for (const marker of Object.values(markers).flatMap(x => Object.values(x).flatMap(x => Object.values(x)))) {
      this.markers.push(marker);
    }
  }

  private refreshMarkerDescriptions() {
    const markerDescriptions = [];

    if (this.team) {
      const markers: { marker: string; display: string; color: string }[] = [];
      for (const character of this.team.characters) {
        markers.push({ marker: character.id, display: character.name, color: 'cyan' });
      }
      markerDescriptions.push({ category: 'Team', markers });
    }

    const jobCategories: { [category: string]: { marker: string; display: string; color: string }[] } = {};
    for (const harvestable of this.harvestables) {
      if (!jobCategories[harvestable.job.name]) {
        jobCategories[harvestable.job.name] = [];
      }

      jobCategories[harvestable.job.name].push({
        marker: harvestable.target.id,
        display: harvestable.target.name,
        color: this.getColor(harvestable.job, harvestable.harvest, harvestable.target),
      });
    }

    for (const category of Object.keys(jobCategories)) {
      markerDescriptions.push({ category, markers: jobCategories[category] });
    }

    for (const category of markerDescriptions) {
      for (const marker of category.markers) {
        const existingCategory = this.markerDescriptions.find(c => c.category === category.category);
        if (!existingCategory || !existingCategory.markers.find(m => m.marker === marker.marker)) {
          this.markerConfiguration[marker.marker] = true;
        }
      }
    }

    this.markerDescriptions = markerDescriptions;
  }
}
