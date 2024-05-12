import { Injectable } from '@angular/core';
import { ActivatedRoute, EventType, Router } from '@angular/router';
import { tap } from 'rxjs';
import { Character } from '../../../api/game-api-client.generated';

@Injectable()
export class CurrentPageService {
  private map: boolean = false;
  private characterId: string | undefined;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.router.events
      .pipe(
        tap(evt => {
          if (evt.type === EventType.NavigationEnd) {
            this.characterId = undefined;
            this.map = false;

            if (this.checkMapOpened(evt.urlAfterRedirects)) {
              this.map = true;
              return;
            }

            const characterId = this.checkCharacterOpened(evt.urlAfterRedirects);
            if (characterId) {
              this.characterId = characterId;
              return;
            }
          }
        }),
      )
      .subscribe();
  }

  openHome() {
    this.router.navigate(['']);
  }

  openMap() {
    this.router.navigate(['maps'], { relativeTo: this.route });
  }

  openCharacter(character: Character) {
    this.router.navigate(['characters', character.id], { relativeTo: this.route });
  }

  isMapOpened() {
    return this.map;
  }

  isAnyCharacterOpened() {
    return !!this.characterId;
  }

  isCharacterOpened(character: Character) {
    return this.characterId === character.id;
  }

  private checkCharacterOpened(url: string) {
    const characterIdPrefix = '/game/characters/';

    if (!url.startsWith(characterIdPrefix)) {
      return undefined;
    }

    const firstSlashAfterCharacterId = url.indexOf('/', characterIdPrefix.length);
    if (firstSlashAfterCharacterId < 0) {
      return url.slice(characterIdPrefix.length);
    } else {
      return url.slice(characterIdPrefix.length, firstSlashAfterCharacterId + 1);
    }
  }

  private checkMapOpened(url: string) {
    return url.startsWith('/game/maps');
  }
}
