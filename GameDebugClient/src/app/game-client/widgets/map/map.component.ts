import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, ElementRef, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { ReplaySubject, debounceTime } from 'rxjs';
import { LocationMinimal } from '../../../../api/game-api-client.generated';

@Component({
  selector: 'app-map',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './map.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MapComponent implements OnInit {
  private static readonly MIN_REDRAW_DELAY_MS = 10;
  private static readonly MIN_REDRAW_SELECTION_DELAY_MS = 1;

  private static readonly MARGIN_SIZE = 20;
  private static readonly RULER_COLOR = 'grey';

  private static readonly CELL_WIDTH = 30;
  private static readonly CELL_HEIGHT = 15;
  private static readonly CELL_BG_COLOR = 'ghostwhite';
  private static readonly AREA_BORDER_COLOR = 'lightgrey';
  private static readonly AREA_BORDER_WIDTH = 2;

  private static readonly GRID_LINE_COLOR = 'lightgrey';
  private static readonly GRID_LINE_WIDTH = 1;

  private static readonly MARKER_SIZE = 8;

  private static readonly ZOOM_MIN = 0.1;
  private static readonly ZOOM_MAX = 5;
  private static readonly ZOOM_STEP = 0.1;

  private static readonly SELECTION_COLOR = 'gold';

  @ViewChild('parent', { static: true }) parent: ElementRef<HTMLDivElement> | undefined;
  @ViewChild('layer0', { static: true }) layer0: ElementRef<HTMLCanvasElement> | undefined;
  @ViewChild('layer1', { static: true }) layer1: ElementRef<HTMLCanvasElement> | undefined;

  @Input({ required: true })
  public get locations(): LocationMinimal[] {
    return this._locations;
  }
  public set locations(value: LocationMinimal[]) {
    this._locations = value;
    this.updateLocationCaches();
    this.redrawSubject.next();
  }
  private _locations: LocationMinimal[] = [];

  @Input()
  public get centerAt(): [number, number] | undefined {
    return this._centerAt;
  }
  public set centerAt(value: [number, number] | undefined) {
    this._centerAt = value;

    if (value) {
      this.center = [...value];
    }

    this.redrawSubject.next();
  }
  private _centerAt: [number, number] | undefined = undefined;

  @Input()
  public get markers(): MapMarker[] {
    return this._markers;
  }
  public set markers(value: MapMarker[]) {
    this._markers = value;
    this.updateMarkerCaches();
    this.redrawSubject.next();
  }
  private _markers: MapMarker[] = [];

  @Input() gridPosition: 'below' | 'above' = 'below';

  protected center: [number, number] = [0, 0];
  protected zoom: number = 1;

  private redrawSubject: ReplaySubject<void> = new ReplaySubject<void>(1);
  private redrawSelectionSubject: ReplaySubject<[number, number]> = new ReplaySubject<[number, number]>(1);

  private lastMouseCoords: [number, number] | undefined;

  private grid: Grid | undefined;
  private cells: Cell[] = [];
  private cellByPosition: { [x: number]: { [y: number]: Cell } } = {};

  private locationsByArea: { [areaId: string]: LocationMinimal[] } = {};
  private locationsByPosition: { [x: number]: { [y: number]: LocationMinimal[] } } = {};
  private locationAreaAdjacencies: { [locationId: string]: CellAdjacency } = {};
  private areaNames: { [areaId: string]: string } = {};
  private areaCenters: { [areaId: string]: [number, number] } = {};
  private markersByPosition: { [x: number]: { [y: number]: MapMarker[] } } = {};

  ngOnInit(): void {
    if (this.parent) {
      new ResizeObserver(() => this.resize()).observe(this.parent.nativeElement);
    }

    this.redrawSubject.pipe(debounceTime(MapComponent.MIN_REDRAW_DELAY_MS)).subscribe(() => {
      this.redraw();

      if (this.lastMouseCoords) {
        this.redrawSelection(this.lastMouseCoords);
      }
    });
    this.redrawSelectionSubject.pipe(debounceTime(MapComponent.MIN_REDRAW_SELECTION_DELAY_MS)).subscribe(coords => {
      this.lastMouseCoords = coords;
      this.redrawSelection(coords);
    });

    this.resize();
  }

  private resize() {
    if (!this.layer0 || !this.layer1 || !this.parent) {
      return;
    }

    this.layer0.nativeElement.width = this.parent.nativeElement.offsetWidth;
    this.layer0.nativeElement.height = this.parent.nativeElement.offsetHeight;

    this.layer1.nativeElement.width = this.parent.nativeElement.offsetWidth;
    this.layer1.nativeElement.height = this.parent.nativeElement.offsetHeight;

    this.updateGridCaches();
    this.redrawSubject.next();
  }

  setZoom(zoom: number) {
    this.zoom = zoom;

    if (this.zoom < MapComponent.ZOOM_MIN) {
      this.zoom = MapComponent.ZOOM_MIN;
    }

    if (this.zoom > MapComponent.ZOOM_MAX) {
      this.zoom = MapComponent.ZOOM_MAX;
    }

    this.updateGridCaches();
    this.redrawSubject.next();
  }

  @HostListener('wheel', ['$event'])
  onScroll(event: WheelEvent) {
    if (event.deltaY == 0) {
      return;
    }

    const step = event.deltaY > 0 ? -MapComponent.ZOOM_STEP : MapComponent.ZOOM_STEP;
    this.setZoom(this.zoom + step);

    event.preventDefault();
  }

  onMouseMove(event: MouseEvent) {
    this.redrawSelectionSubject.next([event.offsetX, event.offsetY]);
  }

  private redraw() {
    if (!this.layer0 || !this.parent || !this.grid) {
      return;
    }

    const canvas = this.layer0.nativeElement;
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      console.error('Could not get canvas context');
      return;
    }

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (this.gridPosition === 'below') {
      this.drawGrid(ctx);
    }

    // 1st pass ------------

    for (const cell of this.cells) {
      this.drawCellBackground(ctx, cell);
    }

    // ---------------------

    // 2nd pass ------------

    for (const cell of this.cells) {
      this.drawAreaBordersAtCell(ctx, cell, MapComponent.AREA_BORDER_COLOR, MapComponent.AREA_BORDER_WIDTH);
    }

    // ---------------------

    // 3rd pass ------------

    for (const cell of this.cells) {
      const markers = this.markersByPosition[cell.location.positionX] ? this.markersByPosition[cell.location.positionX][cell.location.positionY] ?? [] : [];

      if (markers.length > 0) {
        const step = 1 / (markers.length + 1);
        for (let i = 0; i < markers.length; i++) {
          this.drawMarker(ctx, markers[i], (i + 1) * step);
        }
      }
    }

    // ---------------------

    // 4th pass ------------

    if (this.gridPosition === 'above') {
      this.drawGrid(ctx);
    }

    for (const areaId of Object.keys(this.areaCenters)) {
      this.drawAreaName(ctx, areaId, MapComponent.AREA_BORDER_COLOR);
    }

    // ---------------------

    // 5th pass ------------

    this.drawMargins(ctx);

    // ---------------------
  }

  private redrawSelection(coords: [number, number]) {
    if (!this.layer1 || !this.parent || !this.grid) {
      return;
    }

    const canvas = this.layer1.nativeElement;
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      console.error('Could not get canvas context');
      return;
    }

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    const viewCoords: [number, number] = [(coords[0] - this.grid.xMin) / this.grid.cellWidth, (coords[1] - this.grid.yMin) / this.grid.cellHeight];
    const locationCoords = this.computeLocationCoords(this.center, this.grid.size, viewCoords);

    const locationCoordsInt = [Math.floor(locationCoords[0]), Math.floor(locationCoords[1]) + 1];
    const cell = this.cellByPosition[locationCoordsInt[0]] ? this.cellByPosition[locationCoordsInt[0]][locationCoordsInt[1]] : undefined;
    if (!cell) {
      return;
    }

    this.drawCellBorders(ctx, cell, MapComponent.SELECTION_COLOR, 2);
    this.drawAreaBorders(ctx, cell.location.area.id, MapComponent.SELECTION_COLOR, 2);
    this.drawAreaName(ctx, cell.location.area.id, MapComponent.SELECTION_COLOR);
  }

  private drawGrid(ctx: CanvasRenderingContext2D) {
    if (!this.grid) {
      return;
    }

    for (let x = 1; x < this.grid.size[0]; x++) {
      ctx.beginPath();
      ctx.moveTo(this.grid.xMin + x * this.grid.cellWidth, this.grid.yMin);
      ctx.lineTo(this.grid.xMin + x * this.grid.cellWidth, this.grid.yMax);
      ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.GRID_LINE_COLOR;
      ctx.stroke();
    }

    for (let y = 1; y < this.grid.size[1]; y++) {
      ctx.beginPath();
      ctx.moveTo(this.grid.xMin, this.grid.yMin + y * this.grid.cellHeight);
      ctx.lineTo(this.grid.xMax, this.grid.yMin + y * this.grid.cellHeight);
      ctx.lineWidth = MapComponent.GRID_LINE_WIDTH;
      ctx.strokeStyle = MapComponent.GRID_LINE_COLOR;
      ctx.stroke();
    }
  }

  private drawCellBackground(ctx: CanvasRenderingContext2D, cell: Cell) {
    ctx.fillStyle = MapComponent.CELL_BG_COLOR;
    ctx.fillRect(cell.xMin, cell.yMin, cell.width, cell.height);
  }

  private drawCellBorders(ctx: CanvasRenderingContext2D, cell: Cell, lineColor: string, lineWidth: number) {
    ctx.beginPath();
    ctx.moveTo(cell.xMin, cell.yMax);
    ctx.lineTo(cell.xMax, cell.yMax);
    ctx.lineTo(cell.xMax, cell.yMin);
    ctx.lineTo(cell.xMin, cell.yMin);
    ctx.closePath();
    ctx.lineWidth = lineWidth;
    ctx.strokeStyle = lineColor;
    ctx.stroke();
  }

  private drawAreaBorders(ctx: CanvasRenderingContext2D, areaId: string, lineColor: string, lineWidth: number) {
    for (const cell of this.cells) {
      if (cell.location.area.id !== areaId) {
        continue;
      }

      this.drawAreaBordersAtCell(ctx, cell, lineColor, lineWidth);
    }
  }

  private drawAreaBordersAtCell(ctx: CanvasRenderingContext2D, cell: Cell, lineColor: string, lineWidth: number) {
    if (!cell.adjacency.sameAreaBottom) {
      ctx.beginPath();
      ctx.moveTo(cell.xMin, cell.yMax);
      ctx.lineTo(cell.xMax, cell.yMax);
      ctx.lineWidth = lineWidth;
      ctx.strokeStyle = lineColor;
      ctx.stroke();
    }

    if (!cell.adjacency.sameAreaTop) {
      ctx.beginPath();
      ctx.moveTo(cell.xMin, cell.yMin);
      ctx.lineTo(cell.xMax, cell.yMin);
      ctx.lineWidth = lineWidth;
      ctx.strokeStyle = lineColor;
      ctx.stroke();
    }

    if (!cell.adjacency.sameAreaLeft) {
      ctx.beginPath();
      ctx.moveTo(cell.xMin, cell.yMin);
      ctx.lineTo(cell.xMin, cell.yMax);
      ctx.lineWidth = lineWidth;
      ctx.strokeStyle = lineColor;
      ctx.stroke();
    }

    if (!cell.adjacency.sameAreaRight) {
      ctx.beginPath();
      ctx.moveTo(cell.xMax, cell.yMin);
      ctx.lineTo(cell.xMax, cell.yMax);
      ctx.lineWidth = lineWidth;
      ctx.strokeStyle = lineColor;
      ctx.stroke();
    }
  }

  private drawAreaName(ctx: CanvasRenderingContext2D, areaId: string, color: string) {
    if (!this.grid) {
      return;
    }

    const center = this.areaCenters[areaId];
    const viewPosition = this.computeViewCoords(this.center, [this.grid.size[0], this.grid.size[1]], center);

    const text = this.areaNames[areaId] ?? '???';
    const textSize = ctx.measureText(text);

    ctx.lineWidth = 1;
    ctx.fillStyle = color;
    ctx.font = '12px sans-serif';
    ctx.fillText(
      text,
      this.grid.xMin + viewPosition[0] * this.grid.cellWidth - textSize.width / 2,
      this.grid.yMin + viewPosition[1] * this.grid.cellHeight + (textSize.actualBoundingBoxAscent + textSize.actualBoundingBoxDescent) / 2,
    );
  }

  private drawMarker(ctx: CanvasRenderingContext2D, marker: MapMarker, xRelativePos: number = 0.5) {
    if (!this.grid) {
      return;
    }

    const coords = this.computeViewCoords(this.center, [this.grid.size[0], this.grid.size[1]], [marker.positionX, marker.positionY]);

    const x = this.grid.xMin + coords[0] * this.grid.cellWidth + xRelativePos * this.grid.cellWidth;
    const y = (coords[1] + 0.5) * this.grid.cellHeight + this.grid.yMin;

    switch (marker.shape) {
      case 'circle':
        ctx.beginPath();
        ctx.arc(x, y, MapComponent.MARKER_SIZE / 2, 0, 2 * Math.PI);
        ctx.fillStyle = marker.color;
        ctx.fill();
        break;
    }
  }

  private drawMargins(ctx: CanvasRenderingContext2D) {
    if (!this.grid) {
      return;
    }

    ctx.clearRect(0, 0, MapComponent.MARGIN_SIZE, this.grid.height);
    ctx.clearRect(0, 0, this.grid.width, MapComponent.MARGIN_SIZE);

    for (let x = 0; x < this.grid.size[0]; x++) {
      const mapX = this.computeLocationCoords(this.center, [this.grid.size[0], this.grid.size[1]], [x, 0])[0];
      const text = `${mapX}`;
      const textSize = ctx.measureText(text);
      ctx.lineWidth = 1;
      ctx.strokeStyle = MapComponent.RULER_COLOR;
      ctx.strokeText(
        text,
        this.grid.xMin + (x + 0.5) * this.grid.cellWidth - textSize.width / 2,
        MapComponent.MARGIN_SIZE / 2 + (textSize.actualBoundingBoxAscent + textSize.actualBoundingBoxDescent) / 2,
      );
    }

    for (let y = 0; y < this.grid.size[1]; y++) {
      const mapY = this.computeLocationCoords(this.center, [this.grid.size[0], this.grid.size[1]], [0, y])[1];
      const text = `${mapY}`;
      const textSize = ctx.measureText(text);
      ctx.lineWidth = 1;
      ctx.strokeStyle = MapComponent.RULER_COLOR;
      ctx.strokeText(
        text,
        MapComponent.MARGIN_SIZE / 2 - textSize.width / 2,
        this.grid.yMin + (y + 0.5) * this.grid.cellHeight + (textSize.actualBoundingBoxAscent + textSize.actualBoundingBoxDescent) / 2,
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

  private computeViewCoords(viewCenter: [number, number], viewSize: [number, number], mapCoords: [number, number]) {
    const origin = this.computeLocationCoords(viewCenter, viewSize, [0, 0]);
    return [mapCoords[0] - origin[0], origin[1] - mapCoords[1]];
  }

  private updateLocationCaches() {
    this.locationsByArea = {};
    for (const location of this._locations) {
      const area = this.locationsByArea[location.area.id];
      if (area) {
        area.push(location);
      } else {
        this.locationsByArea[location.area.id] = [location];
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

    this.locationAreaAdjacencies = {};
    for (const location of this._locations) {
      const locationsTop = this.locationsByPosition[location.positionX] ? this.locationsByPosition[location.positionX][location.positionY + 1] ?? [] : [];
      const locationsBottom = this.locationsByPosition[location.positionX] ? this.locationsByPosition[location.positionX][location.positionY - 1] ?? [] : [];
      const locationsLeft = this.locationsByPosition[location.positionX - 1] ? this.locationsByPosition[location.positionX - 1][location.positionY] ?? [] : [];
      const locationsRight = this.locationsByPosition[location.positionX + 1] ? this.locationsByPosition[location.positionX + 1][location.positionY] ?? [] : [];

      this.locationAreaAdjacencies[location.id] = {
        sameAreaTop: locationsTop.length > 0 && locationsTop.some(l => l.area.id === location.area.id),
        sameAreaBottom: locationsBottom.length > 0 && locationsBottom.some(l => l.area.id === location.area.id),
        sameAreaLeft: locationsLeft.length > 0 && locationsLeft.some(l => l.area.id === location.area.id),
        sameAreaRight: locationsRight.length > 0 && locationsRight.some(l => l.area.id === location.area.id),
      };
    }

    this.areaNames = {};
    for (const areaId of Object.keys(this.locationsByArea)) {
      const locations = this.locationsByArea[areaId];
      if (locations.length > 0) {
        this.areaNames[areaId] = locations[0].area.name;
      }
    }

    this.areaCenters = {};
    for (const areaId of Object.keys(this.locationsByArea)) {
      const locations = this.locationsByArea[areaId];
      if (locations.length == 0) {
        this.areaCenters[areaId] = [0, 0];
        continue;
      }

      let totalMass = 0;
      let sumX = 0;
      let sumY = 0;
      for (const location of this.locationsByArea[areaId]) {
        const adjacency = this.locationAreaAdjacencies[location.id];

        const mass = 1 + (adjacency.sameAreaTop ? 1 : 0) + (adjacency.sameAreaBottom ? 1 : 0) + (adjacency.sameAreaLeft ? 1 : 0) + (adjacency.sameAreaRight ? 1 : 0);
        totalMass += mass;

        sumX += location.positionX * mass;
        sumY += location.positionY * mass;
      }

      this.areaCenters[areaId] = [sumX / totalMass, sumY / totalMass];
    }

    this.updateGridCaches();
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

  private updateGridCaches() {
    if (!this.layer0) {
      this.grid = undefined;
      return;
    }

    const cellWidth = MapComponent.CELL_WIDTH * this.zoom;
    const cellHeight = MapComponent.CELL_HEIGHT * this.zoom;

    const cellsInViewX = (this.layer0.nativeElement.width - MapComponent.MARGIN_SIZE) / cellWidth;
    const cellsInViewY = (this.layer0.nativeElement.height - MapComponent.MARGIN_SIZE) / cellHeight;

    const cellsInViewXCeiled = Math.ceil(cellsInViewX);
    const cellsInViewYCeiled = Math.ceil(cellsInViewY);

    const totalWidth = cellsInViewXCeiled * cellWidth;
    const totalHeight = cellsInViewYCeiled * cellHeight;

    const startAtX = Math.ceil((this.layer0.nativeElement.width - MapComponent.MARGIN_SIZE - totalWidth) / 2) + MapComponent.MARGIN_SIZE;
    const startAtY = Math.ceil((this.layer0.nativeElement.height - MapComponent.MARGIN_SIZE - totalHeight) / 2) + MapComponent.MARGIN_SIZE;

    this.grid = {
      size: [Math.ceil(cellsInViewX), Math.ceil(cellsInViewY)],
      cellWidth: cellWidth,
      cellHeight: cellHeight,
      width: cellsInViewXCeiled * cellWidth,
      height: cellsInViewYCeiled * cellHeight,
      xMin: startAtX,
      yMin: startAtY,
      xMax: startAtX + totalWidth,
      yMax: startAtY + totalHeight,
    };

    this.cells = [];
    for (let x = 0; x < this.grid.size[0]; x++) {
      for (let y = 0; y < this.grid.size[1]; y++) {
        const position = this.computeLocationCoords(this.center, this.grid.size, [x, y]);

        if (!this.locationsByPosition[position[0]] || !this.locationsByPosition[position[0]][position[1]]) {
          continue;
        }

        const location = this.locationsByPosition[position[0]][position[1]][0];
        const cell = {
          width: this.grid.cellWidth,
          height: this.grid.cellHeight,
          xMin: this.grid.xMin + x * this.grid.cellWidth,
          xMax: this.grid.xMin + (x + 1) * this.grid.cellWidth,
          yMin: this.grid.yMin + y * this.grid.cellHeight,
          yMax: this.grid.yMin + (y + 1) * this.grid.cellHeight,
          xCenter: this.grid.xMin + (x + 0.5) * this.grid.cellWidth,
          yCenter: this.grid.yMin + (y + 0.5) * this.grid.cellHeight,
          location,
          adjacency: this.locationAreaAdjacencies[location.id],
        };

        this.cells.push(cell);

        if (!this.cellByPosition[location.positionX]) {
          this.cellByPosition[location.positionX] = {};
        }

        this.cellByPosition[location.positionX][location.positionY] = cell;
      }
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
  size: [number, number];
  xMin: number;
  xMax: number;
  yMin: number;
  yMax: number;
  width: number;
  height: number;
  cellWidth: number;
  cellHeight: number;
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
  location: LocationMinimal;
  adjacency: CellAdjacency;
}

interface CellAdjacency {
  sameAreaTop: boolean;
  sameAreaBottom: boolean;
  sameAreaLeft: boolean;
  sameAreaRight: boolean;
}
