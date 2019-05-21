import { Component, OnInit } from '@angular/core';
import { BackendService, isSiloFillMessage, SiloFillMessage } from '../iot/backend.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-silo-with-websocket',
  template: `
  <h1 class="mat-headline">Silo with WebSocket</h1>

  <svg id="silo" width="152" height="122">
      <g [class.ok]="fill >= 50" [class.critical]="fill < 50" app-silo [fill]="fill" [maxFill]="100" transform="translate(75, 0)" />
  </svg>
`,
styles: [`
  :host >>> .critical .silo-fill {
    fill: red;
  }

  :host >>> .critical .material-flow {
    stroke: red;
  }

  :host >>> .ok .silo-fill {
    fill: green;
  }

  :host >>> .ok .material-flow {
    stroke: green;
  }
`]})
export class SiloWithWebsocketComponent implements OnInit {
  public fill = 0;

  constructor(private backend: BackendService) { }

  ngOnInit() {
    this.backend.messages$.pipe(
      filter(m => isSiloFillMessage(m) && m.deviceId === 42)
    ).subscribe((m: SiloFillMessage) => this.fill = m.value);
  }
}
