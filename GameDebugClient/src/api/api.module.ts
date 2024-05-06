import { HttpClientModule } from '@angular/common/http';
import { InjectionToken, ModuleWithProviders, NgModule } from '@angular/core';
import { AdminApiStatusApiClient, AdminGameApiClient, AdminPlayersApiClient } from './admin-api-client.generated';
import { GameApiClient, GameApiStatusApiClient, TeamApiClient, TeamCharactersActionsApiClient, TeamCharactersApiClient } from './game-api-client.generated';

const API_BASE_URL: InjectionToken<string> = new InjectionToken<string>('BASE_URL');

@NgModule({
  imports: [HttpClientModule],
  exports: [HttpClientModule],
  providers: [
    AdminApiStatusApiClient,
    AdminPlayersApiClient,
    AdminGameApiClient,
    GameApiStatusApiClient,
    GameApiClient,
    TeamApiClient,
    TeamCharactersApiClient,
    TeamCharactersActionsApiClient,
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
