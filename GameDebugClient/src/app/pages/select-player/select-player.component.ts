import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbDropdownModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { finalize } from 'rxjs';
import { v4 as uuidv4 } from 'uuid';
import { Player, PlayersApiClient } from '../../../api/admin-api-client.generated';
import { SpinnerComponent } from '../../common/spinner/spinner.component';
import { GAME_PATH } from '../../routes';
import { SelectedPlayerService } from './selected-player.service';

@Component({
  selector: 'app-select-player',
  standalone: true,
  templateUrl: './select-player.component.html',
  imports: [CommonModule, NgbDropdownModule, SpinnerComponent],
})
export class SelectPlayerComponent implements OnInit {
  protected loading: boolean = false;
  protected players: Player[] = [];
  protected creating: boolean = false;

  constructor(
    private playersApiClient: PlayersApiClient,
    private selectedPlayerService: SelectedPlayerService,
    private router: Router,
    protected ngbModal: NgbModal,
  ) {}

  ngOnInit(): void {
    this.loading = true;
    this.playersApiClient
      .getPlayers()
      .pipe(finalize(() => (this.loading = false)))
      .subscribe(players => (this.players = players));
  }

  createPlayer(name: string) {
    this.creating = true;
    const guid = uuidv4();
    this.playersApiClient
      .registerPlayer(guid, name)
      .pipe(finalize(() => (this.creating = false)))
      .subscribe(player => this.selectPlayer(player));
  }

  selectPlayer(player: Player) {
    this.selectedPlayerService.set(player);
    this.router.navigateByUrl(GAME_PATH);
  }
}
