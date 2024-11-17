import { Component, computed, Signal } from '@angular/core';
import {
  MeasurementRetrievalService,
  Measurement,
} from '../measurement-retrieval.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-measurement-chart',
  templateUrl: './measurement-chart.component.html',
  styleUrls: ['./measurement-chart.component.css'],
  standalone: true,
  imports: [],
})
export class MeasurementChartComponent {
  public readonly measurements: Signal<Measurement[] | undefined>;
  public readonly yAxis: Signal<{ min: number; max: number }>;
  public readonly dxChart: Signal<string>;
  public readonly dyChart: Signal<string>;

  constructor(private retriever: MeasurementRetrievalService) {
    this.measurements = toSignal(retriever.measurements$);

    this.yAxis = computed(() => {
      const allValues = [
        ...(this.measurements()?.map((m) => m.dx) || [0]),
        ...(this.measurements()?.map((m) => m.dy) || [0]),
      ];

      return {
        min: Math.min(...allValues),
        max: Math.max(...allValues),
      };
    });

    this.dxChart = computed(() => this.measurements()?.map((m, index) => this.getPoint(index, m.dx)).join(' ') || '');
    this.dyChart = computed(() => this.measurements()?.map((m, index) => this.getPoint(index, m.dy)).join(' ') || '');
  }

  private getPoint(index: number, value: number): string {
    return `${index * 10},${100 - ((value - this.yAxis().min) / (this.yAxis().max - this.yAxis().min)) * 100}`;
  }
}
