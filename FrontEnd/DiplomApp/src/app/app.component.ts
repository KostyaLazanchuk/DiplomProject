import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NodeCreateAPI, NodeService } from './services/node.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EdgeService } from './services/edge.service';
import { BinaryConnectionService, CartesianProductService, DijkstraService, FatTreeService, MonteCarloService, PathFindingService, RootedProductService } from './services/advanced.services';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, ReactiveFormsModule ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'DiplomApp';

  nodeForm: FormGroup;
  deleteNodeForm: FormGroup;
  updateNodeNameForm: FormGroup;
  patternForm: FormGroup;
  edgeForm: FormGroup;
  edgeOneToOneForm: FormGroup;
  deleteEdgeForm: FormGroup;
  updateEdgeWeightForm: FormGroup;
  binaryConnectionForm: FormGroup;
  cartesianProductForm: FormGroup;
  dijkstraForm: FormGroup;
  checkAnotherWayForm: FormGroup;
  fatTreeForm: FormGroup;
  monteCarloForm: FormGroup;
  aStarForm: FormGroup;
  rootedProductForm: FormGroup;
  randomNodesForm: FormGroup;
  result: any;

  constructor(
    private nodeService: NodeService, 
    private edgeService: EdgeService,
    private binaryConnectionService: BinaryConnectionService,
    private cartesianProductService: CartesianProductService,
    private dijkstraService: DijkstraService,
    private fatTreeService: FatTreeService,
    private monteCarloService: MonteCarloService,
    private pathFindingService: PathFindingService,
    private rootedProductService: RootedProductService,
    private fb: FormBuilder) {

    this.nodeForm = this.fb.group({
      name: ['', Validators.required],
      position: ['', Validators.required ],
    });

    this.deleteNodeForm = this.fb.group({
      deleteName: ['', Validators.required]
    });

    this.updateNodeNameForm = this.fb.group({
      oldName: ['', Validators.required],
      newName: ['', Validators.required]
    });

    this.patternForm = this.fb.group({
      pattern: ['', Validators.required]
    });

    this.edgeForm = this.fb.group({
      sourceNodeName: ['', Validators.required],
      targetNodeName: ['', Validators.required],
      weight: ['', Validators.required]
    });

    this.edgeOneToOneForm = this.fb.group({
      sourceNodeName: ['', Validators.required],
      targetNodeName: ['', Validators.required],
      weightFromSourceToTarget: ['', Validators.required],
      weightFromTargetToSource: ['', Validators.required]
    });

    this.deleteEdgeForm = this.fb.group({
      nodeName: ['', Validators.required]
    });

    this.updateEdgeWeightForm = this.fb.group({
      nodeName: ['', Validators.required],
      newWeight: ['', Validators.required]
    });

    this.binaryConnectionForm = this.fb.group({
      serverCount: ['', Validators.required]
    });

    this.cartesianProductForm = this.fb.group({
      graph1Pattern: ['', Validators.required],
      graph2Pattern: ['', Validators.required]
    });

    this.dijkstraForm = this.fb.group({
      startNodeName: ['', Validators.required],
      goalNodeName: ['', Validators.required]
    });

    this.checkAnotherWayForm = this.fb.group({
      startNodeName: ['', Validators.required],
      goalNodeName: ['', Validators.required]
    });

    this.fatTreeForm = this.fb.group({
      coreCount: ['', Validators.required]
    });

    this.monteCarloForm = this.fb.group({
      failureProbability: ['', Validators.required],
      iterations: ['', Validators.required]
    });

    this.aStarForm = this.fb.group({
      startNodeName: ['', Validators.required],
      goalNodeName: ['', Validators.required]
    });

    this.rootedProductForm = this.fb.group({
      baseNodeName: ['', Validators.required],
      rootedNodeName: ['', Validators.required]
    });

    this.randomNodesForm = this.fb.group({
      countNode: ['', Validators.required],
      nodeName: ['', Validators.required]
    });
  }

  addNode() {
    if (this.nodeForm.valid) {
      const newNode: NodeCreateAPI = {
        Name: this.nodeForm.value.name,
        Position: this.nodeForm.value.position,
      };

      this.nodeService.addNode(newNode).subscribe(
        response => {
          console.log('Node added:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  deleteNode() {
    if (this.deleteNodeForm.valid) {
      const nodeName = this.deleteNodeForm.value.deleteName;

      this.nodeService.deleteNode(nodeName).subscribe(
        response => {
          console.log('Node deleted:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  countNodes() {
    this.nodeService.countNodes().subscribe(
      response => {
        console.log('Node count:', response);
        this.result = response;
      },
      error => {
        console.error('There was an error!', error);
        this.result = error;
      }
    );
  }

  updateNodeName() {
    if (this.updateNodeNameForm.valid) {
      const oldName = this.updateNodeNameForm.value.oldName;
      const newName = this.updateNodeNameForm.value.newName;

      this.nodeService.updateNodeName(oldName, newName).subscribe(
        response => {
          console.log('Node name updated:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  getNodesByPattern() {
    if (this.patternForm.valid) {
      const pattern = this.patternForm.value.pattern;

      this.nodeService.getNodesByPattern(pattern).subscribe(
        response => {
          console.log('Nodes with pattern:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  deleteAllData() {
    this.nodeService.deleteAllData().subscribe(
      response => {
        console.log('All data deleted:', response);
        this.result = response;
      },
      error => {
        console.error('There was an error!', error);
        this.result = error;
      }
    );
  }

  addEdge() {
    if (this.edgeForm.valid) {
      const { sourceNodeName, targetNodeName, weight } = this.edgeForm.value;

      this.edgeService.addEdge(sourceNodeName, targetNodeName, weight).subscribe(
        response => {
          console.log('Edge added:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  addEdgeOneToOne() {
    if (this.edgeOneToOneForm.valid) {
      const { sourceNodeName, targetNodeName, weightFromSourceToTarget, weightFromTargetToSource } = this.edgeOneToOneForm.value;

      this.edgeService.addEdgeOneToOne(sourceNodeName, targetNodeName, weightFromSourceToTarget, weightFromTargetToSource).subscribe(
        response => {
          console.log('Edge added one-to-one:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  deleteEdgesByNodeName() {
    if (this.deleteEdgeForm.valid) {
      const nodeName = this.deleteEdgeForm.value.nodeName;

      this.edgeService.deleteEdgesByNodeName(nodeName).subscribe(
        response => {
          console.log('Edges deleted:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  updateEdgeWeightByNodeName() {
    if (this.updateEdgeWeightForm.valid) {
      const { nodeName, newWeight } = this.updateEdgeWeightForm.value;

      this.edgeService.updateEdgeWeightByNodeName(nodeName, newWeight).subscribe(
        response => {
          console.log('Edge weights updated:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  countEdges() {
    this.edgeService.countEdges().subscribe(
      response => {
        console.log('Edge count:', response);
        this.result = response;
      },
      error => {
        console.error('There was an error!', error);
        this.result = error;
      }
    );
  }

  generateBinaryConnections() {
    if (this.binaryConnectionForm.valid) {
      const { serverCount } = this.binaryConnectionForm.value;

      this.binaryConnectionService.generateBinaryConnections(serverCount).subscribe(
        response => {
          console.log('Binary connections generated:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  createCartesianProduct() {
    if (this.cartesianProductForm.valid) {
      const { graph1Pattern, graph2Pattern } = this.cartesianProductForm.value;

      this.cartesianProductService.createCartesianProduct(graph1Pattern, graph2Pattern).subscribe(
        response => {
          console.log('Cartesian product created:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  findShortestPathDijkstra() {
    if (this.dijkstraForm.valid) {
      const { startNodeName, goalNodeName } = this.dijkstraForm.value;

      this.dijkstraService.findShortestPath(startNodeName, goalNodeName).subscribe(
        response => {
          console.log('Shortest path found (Dijkstra):', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  checkAnotherWay() {
    if (this.checkAnotherWayForm.valid) {
      const { startNodeName, goalNodeName } = this.checkAnotherWayForm.value;

      this.dijkstraService.checkAnotherWay(startNodeName, goalNodeName).subscribe(
        response => {
          console.log('Another way checked:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  createFatTree() {
    if (this.fatTreeForm.valid) {
      const { coreCount } = this.fatTreeForm.value;

      this.fatTreeService.createFatTree(coreCount).subscribe(
        response => {
          console.log('Fat tree created:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  simulateMonteCarlo() {
    if (this.monteCarloForm.valid) {
      const { failureProbability, iterations } = this.monteCarloForm.value;

      this.monteCarloService.simulate(failureProbability, iterations).subscribe(
        response => {
          console.log('Monte Carlo simulation completed:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  findShortestPathAStar() {
    if (this.aStarForm.valid) {
      const { startNodeName, goalNodeName } = this.aStarForm.value;

      this.pathFindingService.findShortestPathAStar(startNodeName, goalNodeName).subscribe(
        response => {
          console.log('Shortest path found (A*):', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  executeRootedProduct() {
    if (this.rootedProductForm.valid) {
      const { baseNodeName, rootedNodeName } = this.rootedProductForm.value;

      this.rootedProductService.executeRootedProduct(baseNodeName, rootedNodeName).subscribe(
        response => {
          console.log('Rooted product execution completed:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }

  createRandomNodesAndEdges() {
    if (this.randomNodesForm.valid) {
      const { countNode, nodeName } = this.randomNodesForm.value;

      this.nodeService.createRandomNodesAndEdges(countNode, nodeName).subscribe(
        response => {
          console.log('Random nodes and edges created:', response);
          this.result = response;
        },
        error => {
          console.error('There was an error!', error);
          this.result = error;
        }
      );
    }
  }
}
