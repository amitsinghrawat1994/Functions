import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Callback } from './callback/callback';
import { Dashboard } from './dashboard/dashboard';

export const routes: Routes = [
  // { path: '', component: Login },
  // { path: 'login', component: Login },
  // { path: 'callback', component: Callback },
  // // { path: 'dashboard', component: DashboardComponent },
  // { path: '**', redirectTo: '' },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: Dashboard },
  { path: 'callback', component: Callback },
  { path: '**', redirectTo: '/dashboard' },
];
