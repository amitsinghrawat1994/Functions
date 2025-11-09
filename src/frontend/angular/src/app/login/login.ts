import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
// import { AuthService } from '@auth0/auth0-angular';
import { Observable } from 'rxjs';
import { AuthCloudService } from './AuthCloudService';

@Component({
  selector: 'app-login',
  imports: [CommonModule],
  templateUrl: './login.html',
  // styleUrl: './login.css',
  styles: [
    `
      .login-container {
        padding: 20px;
        text-align: center;
      }
      .btn {
        padding: 10px 20px;
        margin: 5px;
        border: none;
        border-radius: 4px;
        cursor: pointer;
      }
      .btn-primary {
        background-color: #007bff;
        color: white;
      }
      .btn-secondary {
        background-color: #6c757d;
        color: white;
      }
    `,
  ],
})
export class Login {
  // constructor(public auth: AuthService, private http: HttpClient) {}

  // login(): void {
  //   this.auth.loginWithRedirect();
  // }

  // logout(): void {
  //   this.auth.logout({ logoutParams: { returnTo: window.location.origin } });
  // }

  // getData(): Observable<any> {
  //   return this.http.get('https://localhost:5001/api/protected');
  // }

  constructor(public auth: AuthCloudService) {}

  login() {
    this.auth.login();
  }

  logout() {
    this.auth.logout();
  }
}
