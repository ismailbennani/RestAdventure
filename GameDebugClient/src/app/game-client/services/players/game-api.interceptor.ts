import type { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { PlayersService } from './players.service';

export const gameApiInterceptor: HttpInterceptorFn = (req, next) => {
  const url = new URL(req.url);

  if (url.pathname.startsWith('/game')) {
    const playersService = inject(PlayersService);

    if (playersService.selected?.apiKey) {
      req = req.clone({ headers: req.headers.set('Authorization', playersService.selected.apiKey) });
    } else {
      console.warn('Could not find selected api key.');
    }
  }

  return next(req);
};
