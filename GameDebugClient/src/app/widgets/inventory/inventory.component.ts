import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { Inventory } from '../../../api/game-api-client.generated';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [CommonModule, NgbTooltipModule],
  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InventoryComponent {
  @Input({ required: true }) inventory: Inventory = null!;
}
