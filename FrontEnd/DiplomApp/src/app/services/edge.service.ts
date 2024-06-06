import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class EdgeService {
  private baseUrl = 'https://localhost:7004/api/Edge';

  constructor(private http: HttpClient) {}

  addEdge(sourceNodeName: string, targetNodeName: string, weight: number): Observable<any> {
    const formData = new FormData();
    formData.append('sourceNodeName', sourceNodeName);
    formData.append('targetNodeName', targetNodeName);
    formData.append('weight', weight.toString());
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.post(`${this.baseUrl}/add`, formData, { headers: headers }).pipe(
      catchError(this.handleError)
    );
  }

  addEdgeOneToOne(sourceNodeName: string, targetNodeName: string, weightFromSourceToTarget: number, weightFromTargetToSource: number): Observable<any> {
    const formData = new FormData();
    formData.append('sourceNodeName', sourceNodeName);
    formData.append('targetNodeName', targetNodeName);
    formData.append('weightFromSourceToTarget', weightFromSourceToTarget.toString());
    formData.append('weightFromTargetToSource', weightFromTargetToSource.toString());
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.post(`${this.baseUrl}/add-one-to-one`, formData, { headers: headers }).pipe(
      catchError(this.handleError)
    );
  }

  deleteEdgesByNodeName(nodeName: string): Observable<any> {
    const formData = new FormData();
    formData.append('NodeName', nodeName);
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.request('delete', `${this.baseUrl}/delete-by-node-name`, { headers: headers, body: formData }).pipe(
      catchError(this.handleError)
    );
  }

  updateEdgeWeightByNodeName(nodeName: string, newWeight: number): Observable<any> {
    const formData = new FormData();
    formData.append('NodeName', nodeName);
    formData.append('NewWeight', newWeight.toString());
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.put(`${this.baseUrl}/update-weight-by-node-name`, formData, { headers: headers }).pipe(
      catchError(this.handleError)
    );
  }

  countEdges(): Observable<any> {
    return this.http.get(`${this.baseUrl}/count`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Client-side errors
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side errors
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(errorMessage);
  }
}
