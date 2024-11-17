import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BASE_URL } from './app.config';
import { Observable, timer, shareReplay } from 'rxjs';
import { switchMap } from 'rxjs/operators';

export type Measurement = {
  timestamp: number;
  dx: number;
  dy: number;
};

@Injectable({
  providedIn: 'root',
})
export class MeasurementRetrievalService {
  public readonly measurements$: Observable<Measurement[]>;

  constructor(
    private httpClient: HttpClient,
    @Inject(BASE_URL) private baseUrl: string
  ) {
    this.measurements$ = timer(0, 250).pipe(
      switchMap(() =>
        this.httpClient.get<Measurement[]>(`${this.baseUrl}/measurements`)
      ),
      shareReplay({ bufferSize: 1, refCount: false })
    );
  }
}
