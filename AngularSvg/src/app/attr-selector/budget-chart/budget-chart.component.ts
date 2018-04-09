import { Component, OnInit, Input } from '@angular/core';

@Component({
  // Note the attribute selector here. This is necessary because Angular
  // would otherwise destroy the SVG code by adding an unknown element.
  selector: '[app-budget-chart]',
  template: `
    <svg:line x1="0" y1="5" [attr.x2]="width" y2="5" class="background-line" />
    <svg:line x1="0" y1="5" [attr.x2]="convertToSize(value)" y2="5" />
  `,
  styles: []
})
export class BudgetChartComponent {
  @Input() maxValue: number = 150;
  @Input() value: number = 0;
  @Input() width: number = 150;

  convertToSize(value: number): number {
    return value * this.width / this.maxValue;
  }
}

// Note that the following component does NOT work. The component above
// shows how it is done correctly.
@Component({
  selector: 'app-broken-budget-chart',
  template: `
    <svg:line x1="0" y1="5" [attr.x2]="width" y2="5" class="background-line" />
    <svg:line x1="0" y1="5" [attr.x2]="convertToSize(value)" y2="5" />
  `,
  styles: []
})
export class BrokenBudgetChartComponent {
  @Input() maxValue: number = 150;
  @Input() value: number = 0;
  @Input() width: number = 150;

  convertToSize(value: number): number {
    return value * this.width / this.maxValue;
  }
}
