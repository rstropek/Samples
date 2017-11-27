import { Observable } from 'rxjs/Rx';
import {HttpClient} from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

export interface ICustomer {
  id: number;
  firstName: string;
  lastName: string;
}

@Component({
  selector: 'app-root',
  template: `
    <h1>Customer List</h1>
    <ul>
      <li *ngFor='let c of customers | async'>{{ c.lastName }}</li>
    </ul>
  `,
  styles: []
})
export class AppComponent implements OnInit {
  public customers: Observable<ICustomer[]>;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.customers = this.http.get<ICustomer[]>('http://localhost:8080/api/customers');
  }
}
