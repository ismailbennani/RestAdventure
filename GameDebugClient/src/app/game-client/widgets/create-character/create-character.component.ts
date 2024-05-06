import { ChangeDetectionStrategy, Component, EventEmitter, Output } from '@angular/core';
import { catchError, finalize, of } from 'rxjs';
import { CharacterClass, CreateCharacterRequest, TeamCharacter, TeamCharactersApiClient } from '../../../../api/game-api-client.generated';

@Component({
  selector: 'app-create-character',
  templateUrl: './create-character.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateCharacterComponent {
  protected creating: boolean = false;

  @Output() character: EventEmitter<TeamCharacter> = new EventEmitter<TeamCharacter>();

  constructor(private teamCharactersApiClient: TeamCharactersApiClient) {}

  protected characterClasses: { value: CharacterClass; display: string }[] = [
    {
      value: CharacterClass.Knight,
      display: 'Knight',
    },
    {
      value: CharacterClass.Mage,
      display: 'Mage',
    },
    {
      value: CharacterClass.Scout,
      display: 'Scout',
    },
    {
      value: CharacterClass.Dealer,
      display: 'Dealer',
    },
  ];

  createCharacter(name: string, cls: string) {
    this.creating = true;
    this.teamCharactersApiClient
      .createCharacter(new CreateCharacterRequest({ name, class: cls as CharacterClass }))
      .pipe(
        finalize(() => (this.creating = false)),
        catchError(e => {
          console.error('Error while creating character.', e);
          return of(undefined);
        }),
      )
      .subscribe(character => {
        if (character) {
          this.character.emit(character);
        }
      });
  }
}
