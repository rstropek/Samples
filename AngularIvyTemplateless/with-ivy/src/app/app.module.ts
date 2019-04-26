import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { StaticComponent } from './static.component';
import { SimpleBindingComponent } from './simple-binding.component';
import { ManualStaticComponent } from './manual-static.component';
import { ManualSimpleBindingComponent } from './manual-simple-binding.component';
import { EventComponent } from './event.component';
import { ManualEventComponent } from './manual-event.component';
import { ParentComponent } from './parent.component';
import { ChildComponent } from './child.component';
import { ManualChildComponent, ManualParentComponent } from './manual-parent-child.component';

@NgModule({
  declarations: [
    AppComponent,
    StaticComponent,
    SimpleBindingComponent,
    EventComponent,
    ParentComponent,
    ChildComponent,
    ManualStaticComponent,
    ManualSimpleBindingComponent,
    ManualEventComponent,
    ManualChildComponent,
    ManualParentComponent
  ],
  imports: [
    BrowserModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
