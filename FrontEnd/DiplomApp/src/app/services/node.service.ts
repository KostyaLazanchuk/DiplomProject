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
  private apiUrl = 'https://localhost:7004/api/node/add';

  constructor(private http: HttpClient) {}

  addNode(nodeAPI: NodeCreateAPI): Observable<any> {
    const formData = new FormData();
    formData.append('Name', nodeAPI.Name);
    formData.append('Position', nodeAPI.Position);
    const headers = new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });


    return this.http.post(this.apiUrl, formData,{headers: headers}).pipe(
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
