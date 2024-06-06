import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class MonteCarloService {
  private baseUrl = 'https://localhost:7004/api/montecarlo';

  constructor(private http: HttpClient) {}

  simulate(failureProbability: number, iterations: number): Observable<any> {
    const formData = new FormData();
    formData.append('FailureProbability', failureProbability.toString());
    formData.append('Iterations', iterations.toString());

    return this.http.post(`${this.baseUrl}/execute`, formData).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(errorMessage);
  }
}
