import { HttpClientModule } from '@angular/common/http';
import { InjectionToken, ModuleWithProviders, NgModule } from '@angular/core';
import { AdminApiStatusApiClient, AdminGameApiClient, AdminGameContentApiClient, AdminGameStateApiClient, AdminPlayersApiClient } from './admin-api-client.generated';
import {
  CharactersApiClient,
  CombatsApiClient,
  CombatsHistoryApiClient,
  CombatsInPreparationApiClient,
  GameApiClient,
  GameApiStatusApiClient,
  JobsHarvestApiClient,
  LocationsApiClient,
  PveApiClient,
  TeamApiClient,
} from './game-api-client.generated';

const API_BASE_URL: InjectionToken<string> = new InjectionToken<string>('BASE_URL');

@NgModule({
  imports: [HttpClientModule],
  exports: [HttpClientModule],
  providers: [
    AdminApiStatusApiClient,
    AdminPlayersApiClient,
    AdminGameApiClient,
    AdminGameContentApiClient,
    AdminGameStateApiClient,
    GameApiStatusApiClient,
    GameApiClient,
    TeamApiClient,
    CharactersApiClient,
    LocationsApiClient,
    JobsHarvestApiClient,
    PveApiClient,
    CombatsApiClient,
    CombatsInPreparationApiClient,
    CombatsHistoryApiClient,
  ],
})
export class ApiModule {
  static forRoot(baseUrl: string): ModuleWithProviders<ApiModule> {
    return {
      ngModule: ApiModule,
      providers: [{ provide: API_BASE_URL, useValue: baseUrl }],
    };
  }
}
