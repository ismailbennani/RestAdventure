import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input, numberAttribute } from '@angular/core';

@Component({
  selector: 'app-progression-bar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './progression-bar.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProgressionBarComponent {
  @Input()
  public get value(): number {
    return this._value;
  }
  public set value(value: number) {
    this._value = value;
    this.refreshCachedValues();
  }
  private _value: number = 0;

  @Input()
  public get maxValue(): number | undefined {
    return this._maxNumber;
  }
  public set maxValue(value: number | undefined) {
    this._maxNumber = value;
    this.refreshCachedValues();
  }
  private _maxNumber: number | undefined = 0;

  @Input() color: 'success' | 'warning' | 'info' | 'danger' = 'danger';
  @Input() label: string | undefined;
  @Input({ transform: numberAttribute }) labelSize: number = 2;
  @Input() displayValue: 'percent' | 'absolute' | undefined = undefined;

  public percent: number = 0;
  public textColor = 'light';

  private refreshCachedValues() {
    this.percent = !this.maxValue || this.maxValue === 0 ? 100 : Math.floor((this.value * 100) / this.maxValue);

    if (this.percent > 45) {
      switch (this.color) {
        case 'success':
        case 'warning':
        case 'danger':
          this.textColor = 'light';
          break;
        case 'info':
          this.textColor = 'dark';
          break;
      }
    } else {
      this.textColor = 'dark';
    }
  }
}
