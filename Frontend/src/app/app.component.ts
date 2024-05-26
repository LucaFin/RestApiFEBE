import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UsersComponent } from './users/users.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [RouterModule, UsersComponent]
})
export class AppComponent {
  title = 'frontend';
}
