import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <app-static></app-static>
    <app-simple-binding></app-simple-binding>
  `,
  styles: []
})
export class AppComponent {
  title = 'without-ivy';
}
