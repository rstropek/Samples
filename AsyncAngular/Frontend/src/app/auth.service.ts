import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Subject, Observable } from 'rxjs';
import { startWith, distinctUntilChanged } from 'rxjs/operators';

// This is a naive implementation of a helper service for authentication.
// Note that in practice you should use a ready-made component for this
// ideally in conjunction with OpenID Connect and Azure Active Directory.
// Here, auth is kept very simple because proper auth is not in scope
// for this demo.

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userNameValue: string;
  private tokenValue: string;
  private signedInStatusObs = new Subject<boolean>();

  constructor(private http: HttpClient) { }

  get token() { return this.tokenValue; }
  get userName() { return this.userNameValue; }
  get isSignedIn() { return this.tokenValue; }

  get signedInStatusChanged$(): Observable<boolean> {
    return this.signedInStatusObs.pipe(startWith(false), distinctUntilChanged());
  }

  login(user: string, password: string) {
    this.http.post(`${environment.signalrBaseUrl}/api/Login`, null,
      { headers: new HttpHeaders().set('Authorization', `Basic ${btoa(`${user}:${password}`)}`) })
      .subscribe(
        (result: any) => {
          this.tokenValue = result.accessToken;
          this.userNameValue = user;
          this.signedInStatusObs.next(true);
        }
      );
  }

  logout() {
    this.tokenValue = null;
    this.userNameValue = null;
    this.signedInStatusObs.next(false);
  }
}
