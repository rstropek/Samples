import {animate, state, style, transition, trigger} from '@angular/animations';
import {Component, OnInit} from '@angular/core';

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
      transition('open => closed', animate('200ms ease-in')),
      transition('closed => open', animate('200ms ease-out'))
    ]),
    trigger('flapStateRight', [
      state('open', style({
        transform: 'translate(2.5px, 0.5px)'
      })),
      state('closed', style({
        transform: 'translate(0, 0.5px)'
      })),
      transition('open => closed', animate('200ms ease-in')),
      transition('closed => open', animate('200ms ease-out'))
    ]),
    trigger('fill', [
      state('open', style({
        transform: 'translate(0px, 3.5px)'
      })),
      state('closed', style({
        transform: 'translate(0, 0px)'
      })),
      transition('open => closed', animate('200ms ease-in')),
      transition('closed => open', animate('200ms ease-out'))
    ]),
    trigger('materialFlow', [
      state('open', style({
        opacity: 1
      })),
      state('closed', style({
        opacity: 0
      })),
      transition('open => closed', animate('200ms ease-in')),
      transition('closed => open', animate('200ms ease-out'))
    ])
  ]
})
export class SiloComponent {
  public isOpen: boolean = false;

  public toggleFlap() {
    this.isOpen = !this.isOpen;
  }
}
