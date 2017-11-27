import { LogInterceptor } from './http-interceptor';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';

import {AppComponent} from './app.component';


@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, HttpClientModule],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: LogInterceptor,
    multi: true,
  }],
  bootstrap: [AppComponent]
})
export class AppModule {
}
