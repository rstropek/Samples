import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-simple-binding',
  template: '<h1>{{ greeting }} {{ target }}</h1><ul><li>First</li><li>Second</li></ul>',
  styles: []
})
export class SimpleBindingComponent implements OnInit {
  greeting = "Hello";
  target = "World";

  constructor() { }

  ngOnInit() {
  }

}
