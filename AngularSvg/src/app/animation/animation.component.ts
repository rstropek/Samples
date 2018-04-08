import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-animation',
  template: `
    <h1 class="mat-headline">Animation</h1>

    <svg id="silo" width="152" height="122">
        <g app-silo />
    </svg>
  `,
  styles: [`
  :host >>> .silo-fill {
    fill: red;
  }`]
})
export class AnimationComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
