import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-static',
  template: '<h1>Hello World</h1><ul><li>First</li><li>Second</li></ul>',
  styles: []
})
export class StaticComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
