import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <app-static></app-static>
    <app-simple-binding></app-simple-binding>
    <app-event></app-event>
    <app-parent></app-parent>
    <app-manual-static></app-manual-static>
    <app-manual-simple-binding></app-manual-simple-binding>
    <app-manual-event></app-manual-event>
    <app-manual-parent></app-manual-parent>
  `,
  styles: []
})
export class AppComponent {
  title = 'with-ivy';
}
