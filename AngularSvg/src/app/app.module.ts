import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { MatToolbarModule, MatMenuModule, MatIconModule, MatButtonModule, MatInputModule, MatInput, MatCheckboxModule, MatTableModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ContextMenuModule } from 'ngx-contextmenu';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { WhatsSvgComponent } from './whats-svg/whats-svg.component';
import { AttrBindingComponent } from './attr-binding/attr-binding.component';
import { ProjectListComponent } from './attr-selector/project-list/project-list.component';
import { BudgetChartComponent } from './attr-selector/budget-chart/budget-chart.component';
import { IotModule } from './iot/iot.module';
import { AnimationComponent } from './animation/animation.component';


@NgModule({
  declarations: [
    AppComponent,
    WhatsSvgComponent,
    AttrBindingComponent,
    ProjectListComponent,
    BudgetChartComponent,
    AnimationComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FlexLayoutModule,
    MatToolbarModule,
    MatMenuModule,
    MatIconModule,
    MatButtonModule,
    MatInputModule,
    MatCheckboxModule,
    MatTableModule,
    BrowserAnimationsModule,
    ContextMenuModule.forRoot(),
    IotModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
