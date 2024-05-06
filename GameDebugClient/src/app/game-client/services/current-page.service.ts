import { Injectable } from '@angular/core';
import { ActivatedRoute, EventType, Router } from '@angular/router';
import { tap } from 'rxjs';
import { TeamCharacter } from '../../../api/game-api-client.generated';

@Injectable()
export class CurrentPageService {
  private characterId: string | undefined;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.router.events
      .pipe(
        tap(evt => {
          if (evt.type === EventType.NavigationEnd) {
            const characterIdPrefix = '/game/characters/';
            if (evt.urlAfterRedirects.startsWith(characterIdPrefix)) {
              const firstSlashAfterCharacterId = evt.urlAfterRedirects.indexOf('/', characterIdPrefix.length);
              if (firstSlashAfterCharacterId < 0) {
                this.characterId = evt.urlAfterRedirects.slice(characterIdPrefix.length);
              } else {
                this.characterId = evt.urlAfterRedirects.slice(characterIdPrefix.length, firstSlashAfterCharacterId + 1);
              }
            } else {
              this.characterId = undefined;
            }
          }
        }),
      )
      .subscribe();
  }

  openHome() {
    this.router.navigate(['']);
  }

  openCharacter(character: TeamCharacter) {
    this.router.navigate(['characters', character.id], { relativeTo: this.route });
  }

  isAnyCharacterOpened() {
    return !!this.characterId;
  }

  isCharacterOpened(character: TeamCharacter) {
    return this.characterId === character.id;
  }
}
