import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { Inventory } from '../../../../api/game-api-client.generated';

@Component({
  selector: 'app-inventory',
  templateUrl: './inventory.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  standalone: true,
})
export class InventoryComponent {
  @Input({ required: true }) inventory: Inventory = null!;
}
