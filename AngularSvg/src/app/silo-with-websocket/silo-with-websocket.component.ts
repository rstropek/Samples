import { Component, OnInit } from '@angular/core';

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

  constructor() { }

  ngOnInit() {
    // Create WebSocket connection.
    const socket = new WebSocket('ws://localhost:3000');

    // Listen for messages
    const that = this;
    socket.addEventListener('message', function (event) {
      that.fill = parseInt(event.data);
    });
  }

}
