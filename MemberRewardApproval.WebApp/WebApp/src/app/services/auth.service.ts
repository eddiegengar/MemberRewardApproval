import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<string | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  login(username: string) {
    localStorage.setItem('username', username);
    this.currentUserSubject.next(username);
  }

  logout() {
    localStorage.removeItem('username');
    this.currentUserSubject.next(null);
  }

  getCurrentUser(): string | null {
    return localStorage.getItem('username');
  }

  isLoggedIn(): boolean {
    return !!this.getCurrentUser();
  }
}
