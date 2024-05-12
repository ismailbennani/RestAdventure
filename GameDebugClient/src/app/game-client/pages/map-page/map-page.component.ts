import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Observable, ReplaySubject, debounceTime, forkJoin, map } from 'rxjs';
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
import { GameService } from '../../services/game.service';
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
  protected markerGroups: { [group: string]: MapMarker[] } = {};
  protected markerCounts: { [name: string]: number } = {};
  protected markerDescriptions: { category: string; markers: { name: string; display: string; marker: { shape: MapMarkerShape; color: string; borderColor?: string } }[] }[] = [];
  protected markerConfiguration: { [name: string]: boolean } = {};
  protected markerCategoryConfiguration: { [category: string]: boolean | 'indeterminate' } = {};

  private team: Team | undefined;
  private jobs: Job[] = [];
  private harvestables: { job: Job; harvest: HarvestableEntityHarvestMinimal; target: StaticObject; instances: Entity[] }[] = [];

  private refreshMarkersSubject: ReplaySubject<void> = new ReplaySubject<void>(1);

  constructor(
    private adminGameContentApiClient: AdminGameContentApiClient,
    private adminGameStateApiClient: AdminGameStateApiClient,
    private gameService: GameService,
    private teamService: TeamService,
  ) {}

  ngOnInit(): void {
    forkJoin({ locations: this.loadLocations(), jobs: this.loadJobs() })
      .pipe(
        map(() => {
          this.teamService.team$.subscribe(team => {
            this.team = team;
            this.refreshMarkersSubject.next();
          });

          this.gameService.state$.pipe(map(() => this.loadHarvestableInstances().subscribe(() => this.refreshMarkersSubject.next()))).subscribe();
        }),
      )
      .subscribe();

    this.refreshMarkersSubject.pipe(debounceTime(100)).subscribe(() => {
      this.refreshMarkerDescriptions();
      this.refreshMarkers();
    });
  }

  protected setEnabled(marker: string, enabled: boolean) {
    this.markerConfiguration[marker] = enabled;
    this.refreshCategoryConfiguration();
    this.refreshMarkers();
  }

  protected setCategoryEnabled(category: string, enabled: boolean) {
    const markers = this.markerDescriptions.find(c => c.category === category)?.markers;
    if (!markers) {
      return;
    }

    for (const marker of markers) {
      this.markerConfiguration[marker.name] = enabled;
    }

    this.markerCategoryConfiguration[category] = enabled;

    this.refreshMarkers();
  }

  private loadLocations() {
    return this.adminGameContentApiClient.searchLocationsMinimal(1, 99999999).pipe(map(locations => (this.locations = locations.items)));
  }

  private loadJobs() {
    return this.adminGameContentApiClient.searchJobs(1, 99999999).pipe(map(jobs => (this.jobs = jobs.items)));
  }

  private loadHarvestableInstances() {
    const work: Observable<{
      job: Job;
      harvest: HarvestableEntityHarvestMinimal;
      target: StaticObject;
      instances: Entity[];
    }>[] = [];
    for (const job of this.jobs) {
      for (const harvest of job.harvests) {
        for (const target of harvest.targets) {
          work.push(
            this.adminGameStateApiClient.searchStaticObjectInstances(target.id, 1, 99999999).pipe(map(instances => ({ job, harvest, target, instances: instances.items }))),
          );
        }
      }
    }

    return forkJoin(work).pipe(
      map(harvestables => {
        this.harvestables = harvestables.sort((h1, h2) => h1.harvest.level - h2.harvest.level);
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

        return { shape: 'circle', color: 'brown', borderColor: undefined };
      case 'herbalist':
        switch (object.name.toLowerCase()) {
          case 'peppermint plant':
            return { shape: 'circle', color: '#228B22', borderColor: undefined };
          case 'lavender plant':
            return { shape: 'circle', color: '#6A5ACD', borderColor: undefined };
          case 'ginseng plant':
            return { shape: 'circle', color: '#8B4513', borderColor: undefined };
          case 'chamomile plant':
            return { shape: 'circle', color: '#F0E68C', borderColor: undefined };
          case 'echinacea plant':
            return { shape: 'circle', color: '#D2691E', borderColor: undefined };
        }

        return { shape: 'circle', color: 'green', borderColor: undefined };
      default:
        return { shape: 'circle', color: 'darkgrey', borderColor: 'black' };
    }
  }

  private refreshMarkers() {
    this.markerGroups = {};
    this.markerCounts = {};

    if (this.team) {
      const teamDescription = this.markerDescriptions.find(d => d.category == 'Team');
      for (const character of this.team.characters) {
        if (!this.markerConfiguration[character.id]) {
          continue;
        }

        const description = teamDescription?.markers.find(c => c.name === character.id);

        this.markerGroups[character.id] = [
          {
            shape: description?.marker.shape ?? 'circle',
            color: description?.marker.color ?? 'grey',
            borderColor: description?.marker.borderColor,
            positionX: character.location.positionX,
            positionY: character.location.positionY,
          },
        ];

        this.markerCounts[character.id] = 1;
      }
    }

    const harvestableMarkers: { [staticObjectId: string]: { [x: number]: { [y: number]: { job: string; marker: MapMarker } } } } = {};
    for (const harvestable of this.harvestables) {
      if (!this.markerConfiguration[harvestable.target.id]) {
        continue;
      }

      if (!this.markerGroups[harvestable.job.id]) {
        this.markerGroups[harvestable.job.id] = [];
      }

      const description = this.markerDescriptions.find(c => c.category === harvestable.job.name)?.markers.find(m => m.name === harvestable.target.id);

      if (!harvestableMarkers[harvestable.target.id]) {
        harvestableMarkers[harvestable.target.id] = {};
        this.markerCounts[harvestable.target.id] = 1;
      }

      for (const instance of harvestable.instances) {
        if (!harvestableMarkers[harvestable.target.id][instance.location.positionX]) {
          harvestableMarkers[harvestable.target.id][instance.location.positionX] = {};
        }

        if (!harvestableMarkers[harvestable.target.id][instance.location.positionX][instance.location.positionY]) {
          harvestableMarkers[harvestable.target.id][instance.location.positionX][instance.location.positionY] = {
            job: harvestable.job.id,
            marker: {
              shape: description?.marker.shape ?? 'circle',
              color: description?.marker.color ?? 'grey',
              borderColor: description?.marker.borderColor,
              alpha: 0.3,
              positionX: instance.location.positionX,
              positionY: instance.location.positionY,
            },
          };
        }

        harvestableMarkers[harvestable.target.id][instance.location.positionX][instance.location.positionY].marker.alpha =
          (harvestableMarkers[harvestable.target.id][instance.location.positionX][instance.location.positionY].marker.alpha ?? 0) + 0.1;

        this.markerCounts[harvestable.target.id] += 1;
      }
    }

    for (const marker of Object.values(harvestableMarkers).flatMap(x => Object.values(x).flatMap(x => Object.values(x)))) {
      this.markerGroups[marker.job].push(marker.marker);
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
    this.refreshCategoryConfiguration();
  }

  private refreshCategoryConfiguration() {
    for (const category of this.markerDescriptions) {
      let allTrue = true;
      let allFalse = true;
      for (const marker of category.markers) {
        if (this.markerConfiguration[marker.name]) {
          allFalse = false;
        } else {
          allTrue = false;
        }
      }

      this.markerCategoryConfiguration[category.category] = allTrue ? true : allFalse ? false : 'indeterminate';
    }
  }
}
