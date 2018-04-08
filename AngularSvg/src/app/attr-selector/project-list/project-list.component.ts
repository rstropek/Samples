import { Component, OnInit } from '@angular/core';

export interface IProjectBudgetStatus {
  projectName: string;
  budget: number;
  currentCosts: number;
}

@Component({
  selector: 'app-project-list',
  templateUrl: 'project-list.component.html',
  styleUrls: ['project-list.component.css']
})
export class ProjectListComponent {
  public projects: IProjectBudgetStatus[] = [];
  public displayedColumns = ['projectName', 'budget', 'costs', 'chart'];

  constructor() {
    for (let i = 0; i < 20; i++) {
      this.projects.push({
        projectName: `Project ${i}`, 
        budget: Math.ceil(Math.random() * 10) * 10000, 
        currentCosts: Math.ceil(Math.random() * 10) * 10000
      });
    }
  }
}
