import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { map } from 'rxjs';
import { AdminGameApiClient } from '../../../../api/admin-api-client.generated';
import { GameState } from '../../../../api/game-api-client.generated';

@Component({
  selector: 'app-simulation',
  templateUrl: './simulation.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SimulationComponent {
  @Input({ required: true }) state: GameState = null!;
  @Output() refresh: EventEmitter<void> = new EventEmitter<void>();

  constructor(private adminGameApiClient: AdminGameApiClient) {}

  play() {
    this.adminGameApiClient
      .startSimulation()
      .pipe(map(_ => this.triggerRefresh()))
      .subscribe();
  }

  pause() {
    this.adminGameApiClient
      .stopSimulation()
      .pipe(map(_ => this.triggerRefresh()))
      .subscribe();
  }

  step() {
    this.adminGameApiClient
      .tickNow()
      .pipe(map(_ => this.triggerRefresh()))
      .subscribe();
  }

  private triggerRefresh() {
    this.refresh.emit();
  }
}
