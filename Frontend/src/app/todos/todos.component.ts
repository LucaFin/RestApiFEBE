import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { ApiService } from '../api.service';
import { Todo } from '../models/todo';

@Component({
  selector: 'app-todos',
  templateUrl: './todos.component.html',
  styleUrls: ['./todos.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatToolbarModule,
    MatPaginatorModule
  ]
})
export class TodosComponent implements OnInit {
  todos: Todo[] = [];
  userId: number;
  totalTodos = 0;
  pageSize = 10;
  pageIndex = 0;

  constructor(private route: ActivatedRoute, private apiService: ApiService) {
    this.userId = +this.route.snapshot.paramMap.get('userId')!;
  }

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.apiService.getTodos(this.userId, this.pageSize, this.pageIndex * this.pageSize).subscribe(data => {
      this.todos = data.items;
      this.totalTodos = data.totalCount;
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageIndex = event.pageIndex;
    this.loadTodos();
  }
}
