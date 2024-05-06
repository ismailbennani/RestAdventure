import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-blank-page',
  templateUrl: './blank-page.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
})
export class BlankPageComponent {}
