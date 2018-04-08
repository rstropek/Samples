import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SiloComponent } from './silo/silo.component';
import { ContextMenuModule } from 'ngx-contextmenu';

@NgModule({
  imports: [
    CommonModule,
    ContextMenuModule
  ],
  declarations: [
    SiloComponent
  ],
  exports: [
    SiloComponent
  ]
})
export class IotModule { }
