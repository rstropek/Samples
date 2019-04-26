import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-event',
  template: '<button (click)="say(\'Hi\')">Say Hi</button>',
  styles: []
})
export class EventComponent implements OnInit {
  public say(message: string) {
    console.log(message);
  }

  constructor() { }

  ngOnInit() {
  }

}
