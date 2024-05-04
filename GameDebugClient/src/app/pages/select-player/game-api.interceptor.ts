import type { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { SelectedPlayerService } from './selected-player.service';

export const gameApiInterceptor: HttpInterceptorFn = (req, next) => {
  const url = new URL(req.url);

  if (url.pathname.startsWith('/game')) {
    const selectedPlayerService = inject(SelectedPlayerService);
    const selectedPlayer = selectedPlayerService.get();

    if (selectedPlayer.apiKey) {
      req = req.clone({ headers: req.headers.set('Authorization', selectedPlayer.apiKey) });
    } else {
      console.warn('Could not find selected api key.');
    }
  }

  return next(req);
};
