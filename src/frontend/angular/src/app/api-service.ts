import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environment';
import { AuthCloudService } from './login/AuthCloudService';

interface UserProfile {
  UserId: string;
  Email: string;
  Name: string;
  EmailVerified: boolean;
  Claims: Array<{ Type: string; Value: string }>;
}

interface ProtectedData {
  Message: string;
  UserId: string;
  Timestamp: Date;
  Data: string[];
}

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  // constructor(private http: HttpClient, private authService: AuthService) {}

  private readonly baseUrl = environment.apiUrl;

  constructor(private authService: AuthCloudService) {}

  // Get user profile from protected API
  async getUserProfile(): Promise<UserProfile> {
    const url = `${this.baseUrl}protected/profile`;
    return await this.authService.callProtectedApi<UserProfile>(url, 'GET');
  }

  // Get protected data
  async getProtectedData(): Promise<ProtectedData> {
    const url = `${this.baseUrl}protected/data`;
    return await this.authService.callProtectedApi<ProtectedData>(url, 'GET');
  }

  // Create protected data
  async createProtectedData(data: string): Promise<any> {
    const url = `${this.baseUrl}protected/data`;
    const requestBody = { Data: data };
    return await this.authService.callProtectedApi<any>(url, 'POST', requestBody);
  }

  // Get user settings
  async getUserSettings(): Promise<any> {
    const url = `${this.baseUrl}user/settings`;
    return await this.authService.callProtectedApi<any>(url, 'GET');
  }

  // Update user settings
  async updateUserSettings(settings: any): Promise<any> {
    const url = `${this.baseUrl}user/settings`;
    return await this.authService.callProtectedApi<any>(url, 'PUT', settings);
  }

  // Get users (admin only)
  async getUsers(): Promise<any[]> {
    const url = `${this.baseUrl}admin/users`;
    return await this.authService.callProtectedApi<any[]>(url, 'GET');
  }

  // Create user (admin only)
  async createUser(userData: any): Promise<any> {
    const url = `${this.baseUrl}admin/users`;
    return await this.authService.callProtectedApi<any>(url, 'POST', userData);
  }
}
