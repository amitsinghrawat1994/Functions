import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environment';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  async getProtectedData() {
    const token = await this.authService.getAccessTokenSilently();
    const headers = { Authorization: `Bearer ${token}` };

    return firstValueFrom(this.http.get(`${environment.apiUrl}protected/data`, { headers }));
  }
}
