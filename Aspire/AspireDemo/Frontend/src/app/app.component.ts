import { HttpClient } from '@angular/common/http';
import { Component, OnInit, WritableSignal, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <button (click)="onLoad()">Load from DB</button>
    @if (result()) {
      <h1>The result is {{result()}}!</h1>
    }

    @if(error()) {
      <h1>Sorry, an error has occurred. Maybe app is not fully up yet?</h1>
    }

    <router-outlet />
  `,
  styles: [],
})
export class AppComponent implements OnInit {
  result: WritableSignal<number | undefined> = signal(undefined);
  error: WritableSignal<boolean> = signal(false);

  constructor(private client: HttpClient) {}

  ngOnInit(): void {
  }
  
  onLoad(): void {
    this.client.get<{ result: number }>("/api/db").subscribe(res => {
      this.result.set(res.result);
      this.error.set(false);
    }, err => {
      this.error.set(true);
    })
  }
}
