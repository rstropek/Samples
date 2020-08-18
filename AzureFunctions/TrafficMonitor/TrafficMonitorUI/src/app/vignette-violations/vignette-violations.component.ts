import { Component, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';

interface IViolation {
  Type: string;
  Nationality: string;
  LicensePlate: string;
  CameraID: number;
}

@Component({
  selector: 'app-vignette-violations',
  templateUrl: 'vignette-violations.component.html',
  styles: [
  ]
})
export class VignetteViolationsComponent implements OnInit {
  private connection: signalR.HubConnection;

  public messages: IViolation[] = [];
  
  constructor() { }

  ngOnInit(): void {
    this.connection = new signalR.HubConnectionBuilder().withUrl('http://localhost:7071/api').build();
    this.connection.on('vignetteViolation', (violation: IViolation) => {
      violation.Type = 'Vignette Violation';
      this.messages.push(violation);
    });
    this.connection.start().catch(err => console.error(err.toString()));
  }

}
