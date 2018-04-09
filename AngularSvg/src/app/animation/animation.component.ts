import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-animation',
  template: `
    <h1 class="mat-headline">Animation</h1>

    <svg id="silo" width="152" height="122">
        <g class="critical" app-silo [fill]="30" [maxFill]="100" />
        <g class="ok" app-silo [fill]="75" [maxFill]="100" transform="translate(75, 0)" />
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
`]
})
export class AnimationComponent {
}
