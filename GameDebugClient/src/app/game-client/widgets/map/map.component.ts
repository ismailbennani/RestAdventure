import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, ElementRef, HostListener, Input, OnInit, ViewChild } from '@angular/core';
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
  private static readonly AREA_LINE_WIDTH = 2;
  private static readonly MARKER_SIZE = 8;
  private static readonly ZOOM_MIN = 0.1;
  private static readonly ZOOM_MAX = 5;
  private static readonly ZOOM_STEP = 0.01;
  private static readonly MIN_REDRAW_DELAY_MS = 10;

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

  @Input()
  public get markers(): MapMarker[] {
    return this._markers;
  }
  public set markers(value: MapMarker[]) {
    this._markers = value;
    this.updateMarkerCaches();
    this.queueRedraw();
  }
  private _markers: MapMarker[] = [];

  protected center: [number, number] = [0, 0];
  protected zoom: number = 1;

  private redrawSubject: ReplaySubject<void> = new ReplaySubject<void>(1);
  private locationsByArea: { [areaName: string]: Location[] } = {};
  private locationsByPosition: { [x: number]: { [y: number]: Location[] } } = {};
  private locationAreaAdjacency: { [locationId: string]: CellAdjacency } = {};
  private markersByPosition: { [x: number]: { [y: number]: MapMarker[] } } = {};

  ngOnInit(): void {
    if (this.parent) {
      new ResizeObserver(() => this.resize()).observe(this.parent.nativeElement);
    }

    this.redrawSubject.pipe(debounceTime(MapComponent.MIN_REDRAW_DELAY_MS)).subscribe(() => this.redraw());

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

  setZoom(zoom: number) {
    this.zoom = zoom;

    if (this.zoom < MapComponent.ZOOM_MIN) {
      this.zoom = MapComponent.ZOOM_MIN;
    }

    if (this.zoom > MapComponent.ZOOM_MAX) {
      this.zoom = MapComponent.ZOOM_MAX;
    }

    this.queueRedraw();
  }

  @HostListener('wheel', ['$event'])
  onScroll(event: WheelEvent) {
    if (event.deltaY == 0) {
      return;
    }

    const step = event.deltaY > 0 ? -MapComponent.ZOOM_STEP : MapComponent.ZOOM_STEP;
    this.setZoom(this.zoom + step);
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

    const cellWidth = MapComponent.CELL_WIDTH * this.zoom;
    const cellHeight = MapComponent.CELL_HEIGHT * this.zoom;

    const cellsInViewX = (canvas.width - MapComponent.MARGIN_SIZE) / cellWidth;
    const cellsInViewY = (canvas.height - MapComponent.MARGIN_SIZE) / cellHeight;

    const cellsInViewXCeiled = Math.ceil(cellsInViewX);
    const cellsInViewYCeiled = Math.ceil(cellsInViewY);
    const cellsInViewCeiled: [number, number] = [cellsInViewXCeiled, cellsInViewYCeiled];

    const totalWidth = cellsInViewXCeiled * cellWidth;
    const totalHeight = cellsInViewYCeiled * cellHeight;

    const startAtX = Math.ceil((canvas.width - MapComponent.MARGIN_SIZE - totalWidth) / 2) + MapComponent.MARGIN_SIZE;
    const startAtY = Math.ceil((canvas.height - MapComponent.MARGIN_SIZE - totalHeight) / 2) + MapComponent.MARGIN_SIZE;

    const grid: Grid = {
      cellWidth: cellWidth,
      cellHeight: cellHeight,
      lineWidth: Math.ceil(cellsInViewX),
      colHeight: Math.ceil(cellsInViewY),
      width: cellsInViewXCeiled * cellWidth,
      height: cellsInViewYCeiled * cellHeight,
      xMin: startAtX,
      yMin: startAtY,
      xMax: startAtX + totalWidth,
      yMax: startAtY + totalHeight,
    };

    const cells: Cell[] = [];

    for (let x = 0; x < cellsInViewXCeiled; x++) {
      for (let y = 0; y < cellsInViewYCeiled; y++) {
        const position = this.computeLocationCoords(this.center, cellsInViewCeiled, [x, y]);

        if (!this.locationsByPosition[position[0]] || !this.locationsByPosition[position[0]][position[1]]) {
          continue;
        }

        const location = this.locationsByPosition[position[0]][position[1]][0];

        cells.push({
          width: grid.cellWidth,
          height: grid.cellHeight,
          xMin: grid.xMin + x * grid.cellWidth,
          xMax: grid.xMin + (x + 1) * grid.cellWidth,
          yMin: grid.yMin + y * grid.cellHeight,
          yMax: grid.yMin + (y + 1) * grid.cellHeight,
          xCenter: grid.xMin + (x + 0.5) * grid.cellWidth,
          yCenter: grid.yMin + (y + 0.5) * grid.cellHeight,
          location,
          adjacency: this.locationAreaAdjacency[location.id],
        });
      }
    }

    this.drawGrid(ctx, grid);

    // 1st pass
    for (const cell of cells) {
      this.drawCellBackground(ctx, cell);
    }

    // 2nd pass
    for (const cell of cells) {
      this.drawCellBorders(ctx, cell);
    }

    // 3rd pass
    for (const cell of cells) {
      const markers = this.markersByPosition[cell.location.positionX] ? this.markersByPosition[cell.location.positionX][cell.location.positionY] ?? [] : [];

      if (markers.length > 0) {
        const step = 1 / (markers.length + 1);
        for (let i = 0; i < markers.length; i++) {
          this.drawMarker(ctx, cell, markers[i], (i + 1) * step);
        }
      }
    }

    this.drawMargins(ctx, grid);
  }

  private drawGrid(ctx: CanvasRenderingContext2D, grid: Grid) {
    for (let x = 1; x < grid.lineWidth; x++) {
      ctx.beginPath();
      ctx.moveTo(grid.xMin + x * grid.cellWidth, grid.yMin);
      ctx.lineTo(grid.xMin + x * grid.cellWidth, grid.yMax);
      ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.GRID_LINE_COLOR;
      ctx.stroke();
    }

    for (let y = 1; y < grid.colHeight; y++) {
      ctx.beginPath();
      ctx.moveTo(grid.xMin, grid.yMin + y * grid.cellHeight);
      ctx.lineTo(grid.xMax, grid.yMin + y * grid.cellHeight);
      ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.GRID_LINE_COLOR;
      ctx.stroke();
    }
  }

  private drawCellBackground(ctx: CanvasRenderingContext2D, cell: Cell) {
    ctx.fillStyle = MapComponent.GRID_LINE_COLOR;
    ctx.fillRect(cell.xMin, cell.yMin, cell.width, cell.height);
  }

  private drawCellBorders(ctx: CanvasRenderingContext2D, cell: Cell) {
    if (!cell.adjacency.sameAreaBottom) {
      ctx.beginPath();
      ctx.moveTo(cell.xMin, cell.yMax);
      ctx.lineTo(cell.xMax, cell.yMax);
      ctx.lineWidth = MapComponent.AREA_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.AREA_LINE_COLOR;
      ctx.stroke();
    }

    if (!cell.adjacency.sameAreaTop) {
      ctx.beginPath();
      ctx.moveTo(cell.xMin, cell.yMin);
      ctx.lineTo(cell.xMax, cell.yMin);
      ctx.lineWidth = MapComponent.AREA_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.AREA_LINE_COLOR;
      ctx.stroke();
    }

    if (!cell.adjacency.sameAreaLeft) {
      ctx.beginPath();
      ctx.moveTo(cell.xMin, cell.yMin);
      ctx.lineTo(cell.xMin, cell.yMax);
      ctx.lineWidth = MapComponent.AREA_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.AREA_LINE_COLOR;
      ctx.stroke();
    }

    if (!cell.adjacency.sameAreaRight) {
      ctx.beginPath();
      ctx.moveTo(cell.xMax, cell.yMin);
      ctx.lineTo(cell.xMax, cell.yMax);
      ctx.lineWidth = MapComponent.AREA_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.AREA_LINE_COLOR;
      ctx.stroke();
    }
  }

  private drawMarker(ctx: CanvasRenderingContext2D, cell: Cell, marker: MapMarker, xRelativePos: number = 0.5) {
    var x = cell.xMin + xRelativePos * cell.width;

    switch (marker.shape) {
      case 'circle':
        ctx.beginPath();
        ctx.arc(x, cell.yCenter, MapComponent.MARKER_SIZE / 2, 0, 2 * Math.PI);
        ctx.fillStyle = marker.color;
        ctx.fill();
        break;
    }
  }

  private drawMargins(ctx: CanvasRenderingContext2D, grid: Grid) {
    ctx.clearRect(0, 0, MapComponent.MARGIN_SIZE, grid.height);
    ctx.clearRect(0, 0, grid.width, MapComponent.MARGIN_SIZE);

    for (let x = 0; x < grid.lineWidth; x++) {
      const mapX = this.computeLocationCoords(this.center, [grid.lineWidth, grid.colHeight], [x, 0])[0];
      const text = `${mapX}`;
      const textSize = ctx.measureText(text);
      ctx.lineWidth = 1;
      ctx.strokeStyle = MapComponent.RULER_COLOR;
      ctx.strokeText(
        text,
        grid.xMin + (x + 0.5) * grid.cellWidth - textSize.width / 2,
        MapComponent.MARGIN_SIZE / 2 + (textSize.actualBoundingBoxAscent + textSize.actualBoundingBoxDescent) / 2,
      );
    }

    for (let y = 0; y < grid.colHeight; y++) {
      const mapY = this.computeLocationCoords(this.center, [grid.lineWidth, grid.colHeight], [0, y])[1];
      const text = `${mapY}`;
      const textSize = ctx.measureText(text);
      ctx.lineWidth = 1;
      ctx.strokeStyle = MapComponent.RULER_COLOR;
      ctx.strokeText(
        text,
        MapComponent.MARGIN_SIZE / 2 - textSize.width / 2,
        grid.yMin + (y + 0.5) * grid.cellHeight + (textSize.actualBoundingBoxAscent + textSize.actualBoundingBoxDescent) / 2,
      );
    }

    ctx.clearRect(0, 0, MapComponent.MARGIN_SIZE, MapComponent.MARGIN_SIZE);

    ctx.beginPath();
    ctx.moveTo(0, MapComponent.MARGIN_SIZE);
    ctx.lineTo(ctx.canvas.width, MapComponent.MARGIN_SIZE);
    ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
    ctx.strokeStyle = MapComponent.RULER_COLOR;
    ctx.stroke();

    ctx.beginPath();
    ctx.moveTo(MapComponent.MARGIN_SIZE, 0);
    ctx.lineTo(MapComponent.MARGIN_SIZE, ctx.canvas.height);
    ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
    ctx.strokeStyle = MapComponent.RULER_COLOR;
    ctx.stroke();
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
      const locationsLeft = this.locationsByPosition[location.positionX - 1] ? this.locationsByPosition[location.positionX - 1][location.positionY] ?? [] : [];
      const locationsRight = this.locationsByPosition[location.positionX + 1] ? this.locationsByPosition[location.positionX + 1][location.positionY] ?? [] : [];

      this.locationAreaAdjacency[location.id] = {
        sameAreaTop: locationsTop.length > 0 && locationsTop.some(l => l.area.id === location.area.id),
        sameAreaBottom: locationsBottom.length > 0 && locationsBottom.some(l => l.area.id === location.area.id),
        sameAreaLeft: locationsLeft.length > 0 && locationsLeft.some(l => l.area.id === location.area.id),
        sameAreaRight: locationsRight.length > 0 && locationsRight.some(l => l.area.id === location.area.id),
      };
    }
  }

  private updateMarkerCaches() {
    this.markersByPosition = {};
    for (const marker of this._markers) {
      let x = this.markersByPosition[marker.positionX];
      if (!x) {
        this.markersByPosition[marker.positionX] = {};
        this.markersByPosition[marker.positionX][marker.positionY] = [marker];
        continue;
      }

      let y = this.markersByPosition[marker.positionX][marker.positionY];
      if (!y) {
        this.markersByPosition[marker.positionX][marker.positionY] = [marker];
        continue;
      }

      this.markersByPosition[marker.positionX][marker.positionY].push(marker);
    }
  }
}

export interface MapMarker {
  shape: 'circle';
  color: string;
  positionX: number;
  positionY: number;
}

interface Grid {
  xMin: number;
  xMax: number;
  yMin: number;
  yMax: number;
  width: number;
  height: number;
  cellWidth: number;
  cellHeight: number;
  lineWidth: number;
  colHeight: number;
}

interface Cell {
  xMin: number;
  xMax: number;
  yMin: number;
  yMax: number;
  xCenter: number;
  yCenter: number;
  width: number;
  height: number;
  location: Location;
  adjacency: CellAdjacency;
}

interface CellAdjacency {
  sameAreaTop: boolean;
  sameAreaBottom: boolean;
  sameAreaLeft: boolean;
  sameAreaRight: boolean;
}
