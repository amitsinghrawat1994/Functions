import { Injectable } from '@angular/core';
// import { AuthService } from '@auth0/auth0-angular';
import { AuthService } from '@auth0/auth0-angular';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthCloudService {
  constructor(private authService: AuthService, private http: HttpClient) {}

  login() {
    this.authService.loginWithRedirect();
  }

  logout() {
    this.authService.logout({ logoutParams: { returnTo: window.location.origin } });
  }

  get isAuthenticated$() {
    return this.authService.isAuthenticated$;
  }
  get user$() {
    return this.authService.user$;
  }

  // Method to get access token for API calls
  getAccessTokenSilently() {
    return this.authService.getAccessTokenSilently();
  }

  // Example API call with authentication
  getProtectedData(): Observable<any> {
    return this.authService
      .getAccessTokenSilently()
      .pipe
      // This is handled automatically by the interceptor if configured
      ();
  }
}
