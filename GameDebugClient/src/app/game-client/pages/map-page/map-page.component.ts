import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Observable, forkJoin, map, switchMap } from 'rxjs';
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
import { MapComponent, MapMarker, MapMarkerShape } from '../../widgets/map/map.component';

@Component({
  selector: 'app-map-page',
  standalone: true,
  templateUrl: './map-page.component.html',
  imports: [CommonModule, MapComponent],
})
export class MapPageComponent implements OnInit {
  protected locations: LocationMinimal[] = [];
  protected markers: MapMarker[] = [];
  protected markerDescriptions: { category: string; markers: { name: string; display: string; marker: { shape: MapMarkerShape; color: string; borderColor?: string } }[] }[] = [];
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
        const work: Observable<{
          job: Job;
          harvest: HarvestableEntityHarvestMinimal;
          target: StaticObject;
          instances: Entity[];
        }>[] = [];
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

  private getMarker(job: Job, harvest: HarvestableEntityHarvestMinimal, object: StaticObject): { shape: MapMarkerShape; color: string; borderColor?: string } {
    switch (job.name.toLowerCase()) {
      case 'forester':
        switch (object.name.toLowerCase()) {
          case 'oak tree':
            return { shape: 'circle', color: '#8B4513', borderColor: undefined };
          case 'pine tree':
            return { shape: 'circle', color: '#228B22', borderColor: undefined };
          case 'maple tree':
            return { shape: 'circle', color: '#FFA500', borderColor: undefined };
          case 'birch tree':
            return { shape: 'circle', color: '#F5F5DC', borderColor: '#483C32' };
          case 'walnut tree':
            return { shape: 'circle', color: '#483C32', borderColor: undefined };
        }

        return { shape: 'circle', color: 'green', borderColor: undefined };
      default:
        return { shape: 'circle', color: 'darkgrey', borderColor: 'black' };
    }
  }

  private refreshMarkers() {
    this.markers = [];

    if (this.team) {
      const teamDescription = this.markerDescriptions.find(d => d.category == 'Team');
      for (const character of this.team.characters) {
        if (!this.markerConfiguration[character.id]) {
          continue;
        }

        const description = teamDescription?.markers.find(c => c.name === character.id);

        this.markers.push({
          shape: description?.marker.shape ?? 'circle',
          color: description?.marker.color ?? 'grey',
          borderColor: description?.marker.borderColor,
          positionX: character.location.positionX,
          positionY: character.location.positionY,
        });
      }
    }

    const harvestableMarkers: { [staticObjectId: string]: { [x: number]: { [y: number]: MapMarker } } } = {};
    for (const harvestable of this.harvestables) {
      if (!this.markerConfiguration[harvestable.target.id]) {
        continue;
      }

      const description = this.markerDescriptions.find(c => c.category === harvestable.job.name)?.markers.find(m => m.name === harvestable.target.id);

      if (!harvestableMarkers[harvestable.target.id]) {
        harvestableMarkers[harvestable.target.id] = {};
      }

      for (const instance of harvestable.instances) {
        if (!harvestableMarkers[harvestable.target.id][instance.location.positionX]) {
          harvestableMarkers[harvestable.target.id][instance.location.positionX] = {};
        }

        if (!harvestableMarkers[harvestable.target.id][instance.location.positionX][instance.location.positionY]) {
          harvestableMarkers[harvestable.target.id][instance.location.positionX][instance.location.positionY] = {
            shape: description?.marker.shape ?? 'circle',
            color: description?.marker.color ?? 'grey',
            borderColor: description?.marker.borderColor,
            alpha: 0,
            positionX: instance.location.positionX,
            positionY: instance.location.positionY,
          };
        }

        harvestableMarkers[harvestable.target.id][instance.location.positionX][instance.location.positionY].alpha =
          (harvestableMarkers[harvestable.target.id][instance.location.positionX][instance.location.positionY].alpha ?? 0) + 0.2;
      }
    }

    for (const marker of Object.values(harvestableMarkers).flatMap(x => Object.values(x).flatMap(x => Object.values(x)))) {
      this.markers.push(marker);
    }
  }

  private refreshMarkerDescriptions() {
    const markerDescriptions = [];

    if (this.team) {
      const markers: { name: string; display: string; marker: { shape: MapMarkerShape; color: string; borderColor?: string } }[] = [];
      for (const character of this.team.characters) {
        markers.push({ name: character.id, display: character.name, marker: { shape: 'circle', color: 'cyan' } });
      }
      markerDescriptions.push({ category: 'Team', markers });
    }

    const jobCategories: { [category: string]: { name: string; display: string; marker: { shape: MapMarkerShape; color: string; borderColor?: string } }[] } = {};
    for (const harvestable of this.harvestables) {
      if (!jobCategories[harvestable.job.name]) {
        jobCategories[harvestable.job.name] = [];
      }

      jobCategories[harvestable.job.name].push({
        name: harvestable.target.id,
        display: harvestable.target.name,
        marker: this.getMarker(harvestable.job, harvestable.harvest, harvestable.target),
      });
    }

    for (const category of Object.keys(jobCategories)) {
      markerDescriptions.push({ category, markers: jobCategories[category] });
    }

    for (const category of markerDescriptions) {
      for (const marker of category.markers) {
        const existingCategory = this.markerDescriptions.find(c => c.category === category.category);
        if (!existingCategory || !existingCategory.markers.find(m => m.name === marker.name)) {
          this.markerConfiguration[marker.name] = true;
        }
      }
    }

    this.markerDescriptions = markerDescriptions;
  }
}
