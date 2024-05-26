import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter, Routes } from '@angular/router';
import { AppComponent } from './app/app.component';
import { UsersComponent } from './app/users/users.component';
import { TodosComponent } from './app/todos/todos.component';
import { importProvidersFrom } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

const routes: Routes = [
  { path: 'users', component: UsersComponent },
  { path: 'todos/:userId', component: TodosComponent },
  { path: '', redirectTo: '/users', pathMatch: 'full' },
];

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    importProvidersFrom(HttpClientModule)
  ]
})
.catch(err => console.error(err));
