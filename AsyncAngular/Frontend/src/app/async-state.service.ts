import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Observable, Subject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../environments/environment';
import { HubConnectionBuilder, HubConnection, HubConnectionState } from "@microsoft/signalr";
import { IOrder } from './app.component';

interface INegotiateResult {
  accessToken: string;
  url: string;
}

@Injectable({
  providedIn: 'root'
})
export class AsyncStateService {
  private connection: HubConnection;
  private orderProcessedSubject = new Subject<IOrder>();

  constructor(private http: HttpClient, public auth: AuthService) {
    // Connect to SignalR service whenever a user signs in.
    this.auth.signedInStatusChanged$
      .pipe(filter(isLoggedIn => isLoggedIn))
      .subscribe(() => this.connect().then(() => console.log('Connected.')));

    // Disconnect from SignalR service whenever a user signs out.
    this.auth.signedInStatusChanged$
      .pipe(filter(isLoggedIn => !isLoggedIn))
      .subscribe(() => this.disconnect());
  }

  get orderProcessed$(): Observable<IOrder> {
    return this.orderProcessedSubject;
  }

  async connect(): Promise<void> {
    this.getSignalRConnection().subscribe(async signalRConn => {
      // Connect to SignalR service
      this.connection = new HubConnectionBuilder()
        .withUrl(signalRConn.url, { accessTokenFactory: () => signalRConn.accessToken })
        .build();

      // In practice, you should probably add some auto-reconnect logic here.
      // this.connection.onclose(() => ... );

      // Listen to calls from server
      this.connection.on('orderProcessed', o => this.orderProcessedSubject.next(o));

      // Start the connection
      await this.connection.start();
    });
  }

  async disconnect(): Promise<void> {
    if (this.connection?.state === HubConnectionState.Connected) {
      await this.connection.stop();
    }
  }

  sayHelloToServerViaSignalR() {
    this.connection.send('hello', 'from', 'client');
  }

  private getSignalRConnection(): Observable<INegotiateResult> {
    // Normally, SignalR would call the negotiate endpoint automatically.
    // Here we call it manually because we want to control the user name
    // for each connection. With this, the server can easily send 
    // messages to specific users.

    const headers = new HttpHeaders()
      .set('x-ms-signalr-userid', this.auth.userName)
      .set('Authorization', `Bearer ${this.auth.token}`);
    return this.http.get<INegotiateResult>(
      `${environment.signalrBaseUrl}/api/negotiate`, { headers: headers }
    );
  }
}
