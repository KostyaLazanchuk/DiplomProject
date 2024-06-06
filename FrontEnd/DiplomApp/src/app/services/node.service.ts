import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface NodeCreateAPI {
  Name: string;
  Position: string;
}

export interface Node {
  Id: string;
  Name: string;
  Position: string;
  CreatedOn: string;
}

@Injectable({
  providedIn: 'root',
})
export class NodeService {
  private baseUrl = 'https://localhost:7004/api/node';

  constructor(private http: HttpClient) {}

  addNode(nodeAPI: NodeCreateAPI): Observable<any> {
    const formData = new FormData();
    formData.append('Name', nodeAPI.Name);
    formData.append('Position', nodeAPI.Position);
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.post(`${this.baseUrl}/add`, formData, { headers: headers }).pipe(
      catchError(this.handleError)
    );
  }

  deleteNode(nodeName: string): Observable<any> {
    const formData = new FormData();
    formData.append('NodeName', nodeName);
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.delete(`${this.baseUrl}/delete`, { headers: headers, body: formData }).pipe(
      catchError(this.handleError)
    );
  }

  countNodes(): Observable<any> {
    return this.http.get(`${this.baseUrl}/count`).pipe(
      catchError(this.handleError)
    );
  }

  updateNodeName(oldName: string, newName: string): Observable<any> {
    const formData = new FormData();
    formData.append('nodeNameInput', oldName);
    formData.append('newNodeName', newName);
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.put(`${this.baseUrl}/update-name`, formData, { headers: headers }).pipe(
      catchError(this.handleError)
    );
  }

  getNodesByPattern(pattern: string): Observable<any> {
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.get(`${this.baseUrl}/nodes-by-pattern`, {
      headers: headers,
      params: { pattern: pattern }
    }).pipe(
      catchError(this.handleError)
    );
  }

  deleteAllData(): Observable<any> {
    return this.http.delete(`${this.baseUrl}/delete-all`).pipe(
      catchError(this.handleError)
    );
  }

  createRandomNodesAndEdges(countNode: number, nodeName: string): Observable<any> {
    const formData = new FormData();
    formData.append('countNode', countNode.toString());
    formData.append('nodeName', nodeName);
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });

    return this.http.post(`${this.baseUrl}/create-random-nodes-and-edges`, formData, { headers: headers }).pipe(
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
