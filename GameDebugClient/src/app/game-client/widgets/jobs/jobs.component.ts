import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { JobInstance } from '../../../../api/game-api-client.generated';
import { ProgressionBarComponent } from '../../../common/spinner/progression-bar/progression-bar.component';

@Component({
  selector: 'app-jobs',
  standalone: true,
  templateUrl: './jobs.component.html',
  imports: [CommonModule, ProgressionBarComponent],
})
export class JobsComponent {
  @Input() jobs: JobInstance[] = [];
}
