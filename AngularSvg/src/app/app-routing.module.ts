import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { WhatsSvgComponent } from './whats-svg/whats-svg.component';
import { AttrBindingComponent } from './attr-binding/attr-binding.component';
import { ProjectListComponent } from './attr-selector/project-list/project-list.component';
import { AnimationComponent } from './animation/animation.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/whats-svg' },
  { path: 'whats-svg', component: WhatsSvgComponent },
  { path: 'attr-binding', component: AttrBindingComponent },
  { path: 'project-list', component: ProjectListComponent },
  { path: 'animation', component: AnimationComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
