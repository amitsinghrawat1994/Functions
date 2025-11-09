import { Component, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Login } from './login/login';
import { AuthCloudService } from './login/AuthCloudService';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Login, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
  styles: [
    `
      .app-container {
        min-height: 100vh;
      }

      .navbar {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 1rem 2rem;
        background-color: #343a40;
        color: white;
      }

      .nav-brand h1 {
        margin: 0;
      }

      .nav-auth {
        display: flex;
        align-items: center;
        gap: 1rem;
      }

      .nav-link {
        color: white;
        text-decoration: none;
        padding: 0.5rem 1rem;
        border-radius: 4px;
      }

      .nav-link:hover {
        background-color: rgba(255, 255, 255, 0.1);
      }

      .btn {
        padding: 8px 16px;
        border: none;
        border-radius: 4px;
        cursor: pointer;
      }

      .btn-primary {
        background-color: #007bff;
        color: white;
      }

      .main-content {
        padding: 2rem;
      }
    `,
  ],
})
export class App {
  constructor(public authService: AuthCloudService, private router: Router) {}

  ngOnInit() {
    // Check authentication status and redirect accordingly
    this.authService.isAuthenticated$.subscribe((isAuthenticated) => {
      if (isAuthenticated) {
        this.router.navigate(['/dashboard']);
      } else {
        this.router.navigate(['/']);
      }
    });
  }

  login() {
    this.authService.login();
  }
}
