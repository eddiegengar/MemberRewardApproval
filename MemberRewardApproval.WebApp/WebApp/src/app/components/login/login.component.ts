import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, MatInputModule, MatButtonModule],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  username = '';

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    if (this.username.trim()) {
      this.authService.login(this.username);
      this.router.navigate(['/reward']);
    }
  }
}
