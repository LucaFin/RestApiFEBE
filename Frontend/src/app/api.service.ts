import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from './models/user';
import { Todo } from './models/todo';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUsers(limit: number, offset: number): Observable<{ totalCount: number, items: User[] }> {
    return this.http.get<{ totalCount: number, items: User[] }>(`${this.apiUrl}/users?limit=${limit}&offset=${offset}`);
  }

  getTodos(userId: number, limit: number, offset: number): Observable<{ totalCount: number, items: Todo[] }> {
    return this.http.get<{ totalCount: number, items: Todo[] }>(`${this.apiUrl}/todos?userId=${userId}&limit=${limit}&offset=${offset}`);
  }
}
