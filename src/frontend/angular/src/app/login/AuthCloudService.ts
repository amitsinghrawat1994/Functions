import { Injectable } from '@angular/core';
// import { AuthService } from '@auth0/auth0-angular';
import { AuthService } from '@auth0/auth0-angular';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, firstValueFrom, Observable, throwError } from 'rxjs';

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

  getAccessTokenSilently() {
    return this.authService.getAccessTokenSilently();
  }

  getUserProfile() {
    return this.authService.user$;
  }
  // Helper method to get authenticated HTTP headers
  private async getAuthHeaders(): Promise<HttpHeaders> {
    try {
      debugger;
      const token = await this.getAccessTokenSilently();
      return new HttpHeaders({
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      });
    } catch (error) {
      console.error('Error getting access token:', error);
      throw error;
    }
  }

  // Method to call protected API
  async callProtectedApi<T>(
    url: string,
    method: 'GET' | 'POST' | 'PUT' | 'DELETE',
    body?: any
  ): Promise<T> {
    try {
      debugger;
      const headers = await this.getAuthHeaders();

      let response: Observable<T>;

      switch (method) {
        case 'GET':
          response = this.http.get<T>(url, { headers });
          break;
        case 'POST':
          response = this.http.post<T>(url, body, { headers });
          break;
        case 'PUT':
          response = this.http.put<T>(url, body, { headers });
          break;
        case 'DELETE':
          response = this.http.delete<T>(url, { headers });
          break;
        default:
          throw new Error('Invalid HTTP method');
      }

      return await firstValueFrom(
        response.pipe(
          catchError((error) => {
            console.error('API call error:', error);
            return throwError(() => error);
          })
        )
      );
    } catch (error) {
      console.error('Authentication error:', error);
      throw error;
    }
  }
}
