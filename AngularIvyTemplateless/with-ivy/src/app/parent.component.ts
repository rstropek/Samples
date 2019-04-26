import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-parent',
  template: '<p>I am the parent, here is my child:</p><app-child [name]="childName"></app-child>',
  styles: []
})
export class ParentComponent implements OnInit {
  childName = "Bar";

  constructor() { }

  ngOnInit() {
  }

}
