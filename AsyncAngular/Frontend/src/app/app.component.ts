import { Component, OnInit, OnDestroy } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth.service';
import { AsyncStateService } from './async-state.service';
import { Subscription } from 'rxjs';

export interface IOrder {
  customerId: number,
  product: string,
  amount: number
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  private orderProcessedSubscription: Subscription;

  orderStatusList: string[] = [];

  constructor(private http: HttpClient, public auth: AuthService, public asyncState: AsyncStateService) {
    // Just a debug output to see if login/logout works
    auth.signedInStatusChanged$.subscribe(status => {
      console.log(`Signed ${status ? 'in' : 'out'}`);
      this.orderStatusList.splice(0, this.orderStatusList.length);
    });
  }

  ngOnInit(): void {
    // Wait for notifications from server regarding processed orders
    this.orderProcessedSubscription = this.asyncState.orderProcessed$.subscribe(o => {
      this.orderStatusList.push(`Order for ${o.amount} items of ${o.product} has been processed.`);
    });
  }

  ngOnDestroy(): void {
    this.orderProcessedSubscription.unsubscribe();
  }

  sendMessage() {
    this.asyncState.sayHelloToServerViaSignalR();
  }

  login(username: string) {
    this.auth.login(username, 'password');
  }

  logout() {
    this.auth.logout();
  }

  sendOrder() {
    this.orderStatusList.splice(0, this.orderStatusList.length);
    const order: IOrder = {
      customerId: 42,
      product: 'Apples',
      amount: 3141
    };
    this.http.post(`${environment.signalrBaseUrl}/api/Order`, order, { headers: new HttpHeaders().set('Authorization', `Bearer ${this.auth.token}`) })
       .subscribe(() => this.orderStatusList.push('Sent order'), (err) => console.error(err));
  }
}
