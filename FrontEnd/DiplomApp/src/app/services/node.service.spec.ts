import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { NodeCreateAPI, NodeService } from './node.service';

describe('NodeService', () => {
  let service: NodeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [NodeService],
    });
    service = TestBed.inject(NodeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should add a node', () => {
    const mockNode: NodeCreateAPI = { Name: 'Test Node', Position: '1' };

    service.addNode(mockNode).subscribe(response => {
      expect(response).toEqual({ Message: 'Node added.', Node: jasmine.any(Object) });
    });

    const req = httpMock.expectOne('https://localhost:7004/api/node/add');
    expect(req.request.method).toBe('POST');
    req.flush({ Message: 'Node added.', Node: {} });
  });

  // Add similar tests for other methods...
});
