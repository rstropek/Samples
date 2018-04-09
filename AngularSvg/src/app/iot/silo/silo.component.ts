import {animate, state, style, transition, trigger} from '@angular/animations';
import {Component, OnInit, Input} from '@angular/core';

@Component({
  selector: '[app-silo]',
  templateUrl: 'silo.component.html',
  styleUrls: ['silo.component.css'],
  animations: [
    trigger('flapStateLeft', [
      state('open', style({
        transform: 'translate(-2.5px, 0.5px)'
      })),
      state('closed', style({
        transform: 'translate(0, 0.5px)'
      })),
      transition('open => closed', animate('400ms ease-in')),
      transition('closed => open', animate('400ms ease-out'))
    ]),
    trigger('flapStateRight', [
      state('open', style({
        transform: 'translate(2.5px, 0.5px)'
      })),
      state('closed', style({
        transform: 'translate(0, 0.5px)'
      })),
      transition('open => closed', animate('400ms ease-in')),
      transition('closed => open', animate('400ms ease-out'))
    ]),
    trigger('fill', [
      state('open', style({
        transform: 'translate(0px, 3.5px)'
      })),
      state('closed', style({
        transform: 'translate(0, 0px)'
      })),
      transition('open => closed', animate('400ms ease-in')),
      transition('closed => open', animate('400ms ease-out'))
    ]),
    trigger('materialFlow', [
      state('open', style({
        opacity: 1
      })),
      state('closed', style({
        opacity: 0
      })),
      transition('open => closed', animate('400ms ease-in')),
      transition('closed => open', animate('400ms ease-out'))
    ])
  ]
})
export class SiloComponent {
  public openingState = 'closed';

  public toggleFlap() {
    this.openingState = (this.openingState == 'open') ? 'closed' : 'open';
  }

  @Input() maxFill: number = 150;
  @Input() fill: number = 0;

  convertToSize(fill: number): number {
    return fill * 80 / this.maxFill;
  }
}
