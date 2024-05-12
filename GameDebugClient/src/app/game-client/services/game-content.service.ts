import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, Subject, map } from 'rxjs';
import { AdminGameContentApiClient, StaticObject } from '../../../api/admin-api-client.generated';

@Injectable({
  providedIn: 'root',
})
export class GameContentService {
  private subjects: { [key: string]: Subject<any> } = {};

  constructor(private adminGameContentApiClient: AdminGameContentApiClient) {}

  public get staticObjects$(): Observable<StaticObject[]> {
    return this.lazyLoad('static-objects', () => this.adminGameContentApiClient.searchStaticObjects(1, 999999999).pipe(map(result => result.items)));
  }

  private lazyLoad<T>(key: string, loadFn: () => Observable<T>): Observable<T> {
    if (this.subjects[key]) {
      return this.subjects[key];
    }

    this.subjects[key] = new ReplaySubject<T>(1);
    loadFn().subscribe(value => this.subjects[key].next(value));
    return this.subjects[key];
  }
}
