import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-attr-binding',
  templateUrl: 'attr-binding.component.html',
  styleUrls: ['attr-binding.component.css']
})
export class AttrBindingComponent {
  public isOn: boolean = false;

  toggleSwitch()
  {
    this.isOn = !this.isOn;
  }

  convertValueToScale(value: number, maxValue: number, width: number): number {
    return value * width / maxValue;
  }
}
