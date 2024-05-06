/// <reference types="@angular/localize" />

import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { ApiModule } from './api/api.module';
import { AppComponent } from './app/app.component';
import { gameApiInterceptor } from './app/game-client/services/players/game-api.interceptor';
import { routes } from './app/routes';

bootstrapApplication(AppComponent, {
  providers: [provideHttpClient(withInterceptors([gameApiInterceptor]), withFetch()), importProvidersFrom(ApiModule.forRoot('https://localhost:7056')), provideRouter(routes)],
}).catch(err => console.error(err));
