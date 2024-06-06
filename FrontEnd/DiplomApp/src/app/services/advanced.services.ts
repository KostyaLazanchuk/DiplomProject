import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class BinaryConnectionService {
  private baseUrl = 'https://localhost:7004/api/binaryconnection';

  constructor(private http: HttpClient) {}

  generateBinaryConnections(serverCount: number): Observable<any> {
    const body = { ServerCount: serverCount };
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });

    return this.http.post(`${this.baseUrl}/generate`, body, { headers: headers }).pipe(
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

@Injectable({
  providedIn: 'root',
})
export class CartesianProductService {
  private baseUrl = 'https://localhost:7004/api/cartesianproduct';

  constructor(private http: HttpClient) {}

  createCartesianProduct(graph1Pattern: string, graph2Pattern: string): Observable<any> {
    const formData = new FormData();
    formData.append('Graph1Pattern', graph1Pattern);
    formData.append('Graph2Pattern', graph2Pattern);

    return this.http.post(`${this.baseUrl}/create`, formData).pipe(
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

@Injectable({
  providedIn: 'root',
})
export class DijkstraService {
  private baseUrl = 'https://localhost:7004/api/dijkstra';

  constructor(private http: HttpClient) {}

  findShortestPath(startNodeName: string, goalNodeName: string): Observable<any> {
    const formData = new FormData();
    formData.append('StartNodeName', startNodeName);
    formData.append('GoalNodeName', goalNodeName);

    return this.http.post(`${this.baseUrl}/find-shortest-path`, formData).pipe(
      catchError(this.handleError)
    );
  }

  checkAnotherWay(startNodeName: string, goalNodeName: string): Observable<any> {
    const formData = new FormData();
    formData.append('StartNodeName', startNodeName);
    formData.append('GoalNodeName', goalNodeName);

    return this.http.post(`${this.baseUrl}/check-another-way`, formData).pipe(
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

@Injectable({
  providedIn: 'root',
})
export class FatTreeService {
  private baseUrl = 'https://localhost:7004/api/fattree';

  constructor(private http: HttpClient) {}

  createFatTree(coreCount: number): Observable<any> {
    const formData = new FormData();
    formData.append('CoreCount', coreCount.toString());

    return this.http.post(`${this.baseUrl}/create`, formData).pipe(
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

@Injectable({
  providedIn: 'root',
})
export class PathFindingService {
  private baseUrl = 'https://localhost:7004/api/pathfinding';

  constructor(private http: HttpClient) {}

  findShortestPathAStar(startNodeName: string, goalNodeName: string): Observable<any> {
    const formData = new FormData();
    formData.append('StartNodeName', startNodeName);
    formData.append('GoalNodeName', goalNodeName);

    return this.http.post(`${this.baseUrl}/find-shortest-path-astar`, formData).pipe(
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

@Injectable({
  providedIn: 'root',
})
export class RootedProductService {
  private baseUrl = 'https://localhost:7004/api/rootedproduct';

  constructor(private http: HttpClient) {}

  executeRootedProduct(baseNodeName: string, rootedNodeName: string): Observable<any> {
    const formData = new FormData();
    formData.append('BaseNodeName', baseNodeName);
    formData.append('RootedNodeName', rootedNodeName);

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
