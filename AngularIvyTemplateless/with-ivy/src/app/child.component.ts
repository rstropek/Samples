import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-child',
  template: '<p>I am the child {{ name }}</p>',
  styles: []
})
export class ChildComponent implements OnInit {
  @Input() name = "Foo";

  constructor() { }

  ngOnInit() {
  }

}
