import { Component } from '@angular/core';
import { finalize, map } from 'rxjs';
import { v4 as uuidv4 } from 'uuid';
import { AdminPlayersApiClient } from '../../../../api/admin-api-client.generated';
import { SpinnerComponent } from '../../../common/spinner/spinner.component';
import { CurrentPageService } from '../../services/current-page.service';
import { GameService } from '../../services/game.service';
import { PlayersService } from '../../services/players/players.service';

@Component({
  selector: 'app-players',
  templateUrl: './players.component.html',
  standalone: true,
  imports: [SpinnerComponent],
})
export class PlayersComponent {
  protected inCreation: boolean = false;
  protected creating: boolean = false;

  constructor(
    protected currentPageService: CurrentPageService,
    protected gameService: GameService,
    protected playersService: PlayersService,
    protected playersApiClient: AdminPlayersApiClient,
  ) {}

  protected createPlayer(name: string) {
    var id = uuidv4();
    this.playersApiClient
      .registerPlayer(id, name)
      .pipe(
        map(player => this.playersService.refreshAndSelect(player.id)),
        finalize(() => {
          this.creating = false;
          this.inCreation = false;
        }),
      )
      .subscribe();
  }
}
