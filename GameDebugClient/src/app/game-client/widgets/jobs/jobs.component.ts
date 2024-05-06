import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { JobInstance } from '../../../../api/game-api-client.generated';

@Component({
  selector: 'app-jobs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './jobs.component.html',
})
export class JobsComponent {
  @Input() jobs: JobInstance[] = [];
}
