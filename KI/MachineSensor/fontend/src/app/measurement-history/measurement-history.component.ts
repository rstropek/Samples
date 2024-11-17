import { Component, inject, Signal } from '@angular/core';
import { Measurement, MeasurementRetrievalService } from '../measurement-retrieval.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { DecimalPipe } from '@angular/common';
import { MeasurementChartComponent } from "../measurement-chart/measurement-chart.component";

@Component({
  selector: 'app-measurement-history',
  standalone: true,
  imports: [DecimalPipe, MeasurementChartComponent],
  templateUrl: './measurement-history.component.html',
  styleUrl: './measurement-history.component.css'
})
export class MeasurementHistoryComponent {
  public readonly measurements: Signal<Measurement[] | undefined>;

  constructor(private retriever: MeasurementRetrievalService) {
    this.measurements = toSignal(retriever.measurements$);
  }
}
