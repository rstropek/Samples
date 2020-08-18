import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { VignetteViolationsComponent } from './vignette-violations/vignette-violations.component';

const routes: Routes = [
  { path: 'vignette-violations', component: VignetteViolationsComponent },
  { path: '', pathMatch: 'full', redirectTo: '/vignette-violations' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
