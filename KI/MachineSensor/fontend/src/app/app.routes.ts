import { Routes } from '@angular/router';
import { MeasurementHistoryComponent } from './measurement-history/measurement-history.component';
import { MeasureComponent } from './measure/measure.component';

export const routes: Routes = [
  { path: '', redirectTo: 'history', pathMatch: 'full' },
  { path: 'history', component: MeasurementHistoryComponent },
  { path: 'measure', component: MeasureComponent }
];
