import {NgModule} from '@angular/core';
import {FlexLayoutModule} from '@angular/flex-layout';
import {MatButtonModule, MatCheckboxModule, MatIconModule, MatInputModule, MatMenuModule, MatTableModule, MatToolbarModule,} from '@angular/material';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ContextMenuModule} from 'ngx-contextmenu';

import {AnimationComponent} from './animation/animation.component';
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {AttrBindingComponent} from './attr-binding/attr-binding.component';
import {BudgetChartComponent, BrokenBudgetChartComponent} from './attr-selector/budget-chart/budget-chart.component';
import {ProjectListComponent} from './attr-selector/project-list/project-list.component';
import {IotModule} from './iot/iot.module';
import {WhatsSvgComponent} from './whats-svg/whats-svg.component';
import { SiloWithWebsocketComponent } from './silo-with-websocket/silo-with-websocket.component';

@NgModule({
  declarations: [
    AppComponent, WhatsSvgComponent, AttrBindingComponent, ProjectListComponent,
    BudgetChartComponent, AnimationComponent, BrokenBudgetChartComponent, SiloWithWebsocketComponent
  ],
  imports: [
    BrowserModule, AppRoutingModule, FlexLayoutModule, MatToolbarModule,
    MatMenuModule, MatIconModule, MatButtonModule, MatInputModule,
    MatCheckboxModule, MatTableModule, BrowserAnimationsModule,
    ContextMenuModule.forRoot(), IotModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
}
