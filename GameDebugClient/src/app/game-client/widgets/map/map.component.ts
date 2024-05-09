import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ReplaySubject, debounceTime } from 'rxjs';
import { Location } from '../../../../api/game-api-client.generated';

@Component({
  selector: 'app-map',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './map.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MapComponent implements OnInit {
  private static readonly MARGIN_SIZE = 20;
  private static readonly CELL_WIDTH = 30;
  private static readonly CELL_HEIGHT = 15;
  private static readonly RULER_COLOR = 'grey';
  private static readonly GRID_LINE_COLOR = 'lightgrey';
  private static readonly GRID_LINE_WIDTH = 1;
  private static readonly AREA_LINE_COLOR = 'grey';
  private static readonly AREA_LINE_WIDTH = 4;

  @ViewChild('parent', { static: true }) parent: ElementRef<HTMLDivElement> | undefined;
  @ViewChild('canvas', { static: true }) canvas: ElementRef<HTMLCanvasElement> | undefined;

  @Input({ required: true })
  public get locations(): Location[] {
    return this._locations;
  }
  public set locations(value: Location[]) {
    this._locations = value;
    this.updateLocationCaches();
    this.queueRedraw();
  }
  private _locations: Location[] = [];

  @Input()
  public get centerAt(): [number, number] | undefined {
    return this._centerAt;
  }
  public set centerAt(value: [number, number] | undefined) {
    this._centerAt = value;
    this.queueRedraw();
  }
  private _centerAt: [number, number] | undefined = undefined;

  protected center: [number, number] = [0, 0];

  private redrawSubject: ReplaySubject<void> = new ReplaySubject<void>(1);
  private locationsByArea: { [areaName: string]: Location[] } = {};
  private locationsByPosition: { [x: number]: { [y: number]: Location[] } } = {};
  private locationAreaAdjacency: { [locationId: string]: { sameAreaTop: boolean; sameAreaBottom: boolean; sameAreaLeft: boolean; sameAreaRight: boolean } } = {};

  ngOnInit(): void {
    if (this.parent) {
      new ResizeObserver(() => this.resize()).observe(this.parent.nativeElement);
    }

    this.redrawSubject.pipe(debounceTime(100)).subscribe(() => this.redraw());

    this.resize();
  }

  private queueRedraw() {
    this.redrawSubject.next();
  }

  private resize() {
    if (!this.canvas || !this.parent) {
      return;
    }

    this.canvas.nativeElement.width = this.parent.nativeElement.offsetWidth;
    this.canvas.nativeElement.height = this.parent.nativeElement.offsetHeight;

    this.queueRedraw();
  }

  private redraw() {
    if (!this.canvas || !this.parent) {
      return;
    }

    if (this.centerAt) {
      this.center = [...this.centerAt];
    }

    const canvas = this.canvas.nativeElement;
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      console.error('Could not get canvas context');
      return;
    }

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const cellsInViewX = (canvas.width - MapComponent.MARGIN_SIZE) / MapComponent.CELL_WIDTH;
    const cellsInViewY = (canvas.height - MapComponent.MARGIN_SIZE) / MapComponent.CELL_HEIGHT;

    const cellsInViewXCeiled = Math.ceil(cellsInViewX);
    const cellsInViewYCeiled = Math.ceil(cellsInViewY);
    const cellsInViewCeiled: [number, number] = [cellsInViewXCeiled, cellsInViewYCeiled];

    const totalWidth = cellsInViewXCeiled * MapComponent.CELL_WIDTH;
    const totalHeight = cellsInViewYCeiled * MapComponent.CELL_HEIGHT;

    const startAtX = Math.ceil((canvas.width - MapComponent.MARGIN_SIZE - totalWidth) / 2) + MapComponent.MARGIN_SIZE;
    const startAtY = Math.ceil((canvas.height - MapComponent.MARGIN_SIZE - totalHeight) / 2) + MapComponent.MARGIN_SIZE;

    for (let x = 1; x < cellsInViewXCeiled; x++) {
      ctx.beginPath();
      ctx.moveTo(startAtX + x * MapComponent.CELL_WIDTH, startAtY);
      ctx.lineTo(startAtX + x * MapComponent.CELL_WIDTH, startAtY + totalHeight);
      ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.GRID_LINE_COLOR;
      ctx.stroke();
    }

    for (let y = 1; y < cellsInViewYCeiled; y++) {
      ctx.beginPath();
      ctx.moveTo(startAtX, startAtY + y * MapComponent.CELL_HEIGHT);
      ctx.lineTo(startAtX + totalWidth, startAtY + y * MapComponent.CELL_HEIGHT);
      ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.GRID_LINE_COLOR;
      ctx.stroke();
    }

    for (let x = 0; x < cellsInViewXCeiled; x++) {
      for (let y = 0; y < cellsInViewYCeiled; y++) {
        const position = this.computeLocationCoords(this.center, cellsInViewCeiled, [x, y]);

        if (!this.locationsByPosition[position[0]] || !this.locationsByPosition[position[0]][position[1]]) {
          continue;
        }

        const location = this.locationsByPosition[position[0]][position[1]];
        const adjacency = this.locationAreaAdjacency[location[0].id];
        if (!adjacency.sameAreaBottom) {
          ctx.beginPath();
          ctx.moveTo(startAtX + x * MapComponent.CELL_WIDTH, startAtY + (y + 1) * MapComponent.CELL_HEIGHT);
          ctx.lineTo(startAtX + (x + 1) * MapComponent.CELL_WIDTH, startAtY + (y + 1) * MapComponent.CELL_HEIGHT);
          ctx.lineWidth = MapComponent.AREA_LINE_WIDTH;
          ctx.strokeStyle = MapComponent.AREA_LINE_COLOR;
          ctx.stroke();
        }

        if (!adjacency.sameAreaTop) {
          ctx.beginPath();
          ctx.moveTo(startAtX + x * MapComponent.CELL_WIDTH, startAtY + y * MapComponent.CELL_HEIGHT);
          ctx.lineTo(startAtX + (x + 1) * MapComponent.CELL_WIDTH, startAtY + y * MapComponent.CELL_HEIGHT);
          ctx.lineWidth = MapComponent.AREA_LINE_WIDTH;
          ctx.strokeStyle = MapComponent.AREA_LINE_COLOR;
          ctx.stroke();
        }

        if (!adjacency.sameAreaLeft) {
          ctx.beginPath();
          ctx.moveTo(startAtX + x * MapComponent.CELL_WIDTH, startAtY + y * MapComponent.CELL_HEIGHT);
          ctx.lineTo(startAtX + x * MapComponent.CELL_WIDTH, startAtY + (y + 1) * MapComponent.CELL_HEIGHT);
          ctx.lineWidth = MapComponent.AREA_LINE_WIDTH;
          ctx.strokeStyle = MapComponent.AREA_LINE_COLOR;
          ctx.stroke();
        }

        if (!adjacency.sameAreaRight) {
          ctx.beginPath();
          ctx.moveTo(startAtX + (x + 1) * MapComponent.CELL_WIDTH, startAtY + y * MapComponent.CELL_HEIGHT);
          ctx.lineTo(startAtX + (x + 1) * MapComponent.CELL_WIDTH, startAtY + (y + 1) * MapComponent.CELL_HEIGHT);
          ctx.lineWidth = MapComponent.AREA_LINE_WIDTH;
          ctx.strokeStyle = MapComponent.AREA_LINE_COLOR;
          ctx.stroke();
        }

        ctx.fillStyle = MapComponent.GRID_LINE_COLOR;
        ctx.fillRect(startAtX + x * MapComponent.CELL_WIDTH, startAtY + y * MapComponent.CELL_HEIGHT, MapComponent.CELL_WIDTH, MapComponent.CELL_HEIGHT);
      }
    }

    ctx.clearRect(0, 0, MapComponent.MARGIN_SIZE, totalHeight);
    ctx.clearRect(0, 0, totalWidth, MapComponent.MARGIN_SIZE);

    ctx.beginPath();
    ctx.moveTo(0, MapComponent.MARGIN_SIZE);
    ctx.lineTo(canvas.width, MapComponent.MARGIN_SIZE);
    ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
    ctx.strokeStyle = MapComponent.RULER_COLOR;
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(MapComponent.MARGIN_SIZE, 0);
    ctx.lineTo(MapComponent.MARGIN_SIZE, canvas.height);
    ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
    ctx.strokeStyle = MapComponent.RULER_COLOR;
    ctx.stroke();

    for (let x = 0; x < cellsInViewXCeiled; x++) {
      const mapX = this.computeLocationCoords(this.center, cellsInViewCeiled, [x, 0])[0];
      const text = `${mapX}`;
      const textSize = ctx.measureText(text);
      ctx.strokeStyle = MapComponent.RULER_COLOR;
      ctx.strokeText(
        text,
        startAtX + (x + 0.5) * MapComponent.CELL_WIDTH - textSize.width / 2,
        MapComponent.MARGIN_SIZE / 2 + (textSize.actualBoundingBoxAscent + textSize.actualBoundingBoxDescent) / 2,
      );
    }

    for (let y = 0; y < cellsInViewYCeiled; y++) {
      const mapY = this.computeLocationCoords(this.center, cellsInViewCeiled, [0, y])[1];
      const text = `${mapY}`;
      const textSize = ctx.measureText(text);
      ctx.strokeStyle = MapComponent.RULER_COLOR;
      ctx.strokeText(
        text,
        MapComponent.MARGIN_SIZE / 2 - textSize.width / 2,
        startAtY + (y + 0.5) * MapComponent.CELL_HEIGHT + (textSize.actualBoundingBoxAscent + textSize.actualBoundingBoxDescent) / 2,
      );
    }
  }

  private computeLocationCoords(viewCenter: [number, number], viewSize: [number, number], viewCoords: [number, number]) {
    const displacementX = viewCenter[0] - Math.floor(viewSize[0] / 2);
    const displacementY = viewCenter[1] + Math.floor(viewSize[1] / 2);
    return [viewCoords[0] + displacementX, displacementY - viewCoords[1]];
  }

  private updateLocationCaches() {
    this.locationsByArea = {};
    for (const location of this._locations) {
      const area = this.locationsByArea[location.area.name];
      if (area) {
        area.push(location);
      } else {
        this.locationsByArea[location.area.name] = [location];
      }
    }

    this.locationsByPosition = {};
    for (const location of this._locations) {
      let x = this.locationsByPosition[location.positionX];
      if (!x) {
        this.locationsByPosition[location.positionX] = {};
        this.locationsByPosition[location.positionX][location.positionY] = [location];
        continue;
      }

      let y = this.locationsByPosition[location.positionX][location.positionY];
      if (!y) {
        this.locationsByPosition[location.positionX][location.positionY] = [location];
        continue;
      }

      this.locationsByPosition[location.positionX][location.positionY].push(location);
    }

    this.locationAreaAdjacency = {};
    for (const location of this._locations) {
      const locationsTop = this.locationsByPosition[location.positionX] ? this.locationsByPosition[location.positionX][location.positionY + 1] ?? [] : [];
      const locationsBottom = this.locationsByPosition[location.positionX] ? this.locationsByPosition[location.positionX][location.positionY - 1] ?? [] : [];
      const locationsLeft = this.locationsByPosition[location.positionX + 1] ? this.locationsByPosition[location.positionX + 1][location.positionY] ?? [] : [];
      const locationsRight = this.locationsByPosition[location.positionX - 1] ? this.locationsByPosition[location.positionX - 1][location.positionY] ?? [] : [];

      this.locationAreaAdjacency[location.id] = {
        sameAreaTop: locationsTop.length > 0 && locationsTop.some(l => l.area.id === location.area.id),
        sameAreaBottom: locationsBottom.length > 0 && locationsBottom.some(l => l.area.id === location.area.id),
        sameAreaLeft: locationsLeft.length > 0 && locationsLeft.some(l => l.area.id === location.area.id),
        sameAreaRight: locationsRight.length > 0 && locationsRight.some(l => l.area.id === location.area.id),
      };
    }
  }
}
