import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NodeCreateAPI, NodeService } from './services/node.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, ReactiveFormsModule ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'DiplomApp';

  nodeForm: FormGroup;
  constructor(private nodeService: NodeService, private fb: FormBuilder) {
    this.nodeForm = this.fb.group({
      name: ['', Validators.required],
      position: ['', Validators.required ],
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
        },
        error => {
          console.error('There was an error!', error);
        }
      );
    }
  }
}
