import { HttpClient, HttpHeaders } from '@angular/common/http';
import {Component, OnInit} from '@angular/core';
import {Observable} from 'rxjs/Rx';

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
    <button (click)="addCustomer()">Add Customer</button>
  `,
  styles: []
})
export class AppComponent implements OnInit {
  public customers: Observable<ICustomer[]>;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.refreshCustomers();
  }

  private refreshCustomers() {
    this.customers =
        this.http.get<ICustomer[]>('http://localhost:8080/api/customers');
  }

  public async addCustomer() {
    const response = await this.http.post<ICustomer>(
      'http://localhost:8080/api/customers',
      {id: 99, firstName: 'Foo', lastName: 'Bar'}).toPromise();

    console.log(`Customer with id ${response.id} has been added.`);

    this.refreshCustomers();
  }
}
