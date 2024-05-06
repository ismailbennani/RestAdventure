import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TeamCharacter } from '../../../api/game-api-client.generated';

@Injectable()
export class CurrentPageService {
  private characterId: string | undefined;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
  ) {}

  openCharacter(character: TeamCharacter) {
    this.router.navigate(['characters', character.id], { relativeTo: this.route });
  }

  setOpenedCharacter(character: TeamCharacter) {
    this.clear();
    this.characterId = character.id;
  }

  isAnyCharacterOpened() {
    return !!this.characterId;
  }

  isCharacterOpened(character: TeamCharacter) {
    return this.characterId === character.id;
  }

  private clear() {
    this.characterId = undefined;
  }
}
