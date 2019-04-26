import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { StaticComponent } from './static.component';
import { SimpleBindingComponent } from './simple-binding.component';

@NgModule({
  declarations: [
    AppComponent,
    StaticComponent,
    SimpleBindingComponent
  ],
  imports: [
    BrowserModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
