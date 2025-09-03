import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RewardComponent } from './components/reward/reward.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'reward', component: RewardComponent },
  { path: '**', redirectTo: '/login' },
];
