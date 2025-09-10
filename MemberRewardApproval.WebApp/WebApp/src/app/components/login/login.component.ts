import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common'; // ✅ import CommonModule
import { MsalWrapperService } from '../../services/msal.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule], // ✅ add CommonModule here
  templateUrl: './login.component.html',
})
export class LoginComponent {
  isLoading = false;
  errorMessage = '';

  constructor(private msal: MsalWrapperService, private router: Router) {}

  async login() {
    this.isLoading = true;
    this.errorMessage = '';

    try {
      await this.msal.login();
      this.router.navigate(['/reward']);
    } catch (err) {
      console.error('Login failed:', err);
      this.errorMessage = 'Login failed. Please try again.';
    } finally {
      this.isLoading = false;
    }
  }
}
