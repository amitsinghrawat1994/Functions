import { Component } from '@angular/core';
import { AuthCloudService } from '../login/AuthCloudService';
import { ApiService } from '../api-service';
import { Router } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-dashboard',
  imports: [DatePipe, FormsModule, CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
  styles: [
    `
      .dashboard-container {
        padding: 20px;
        max-width: 1200px;
        margin: 0 auto;
      }

      .dashboard-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 30px;
        padding-bottom: 20px;
        border-bottom: 1px solid #eee;
      }

      .dashboard-content {
        display: grid;
        gap: 30px;
      }

      section {
        padding: 20px;
        border: 1px solid #ddd;
        border-radius: 8px;
        background-color: #f9f9f9;
      }

      .btn {
        padding: 8px 16px;
        margin: 5px;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-size: 14px;
      }

      .btn-primary {
        background-color: #007bff;
        color: white;
      }

      .btn-secondary {
        background-color: #6c757d;
        color: white;
      }

      .form-input {
        padding: 8px;
        margin: 5px;
        border: 1px solid #ccc;
        border-radius: 4px;
      }

      .result {
        margin-top: 10px;
        padding: 10px;
        background-color: #d4edda;
        border: 1px solid #c3e6cb;
        border-radius: 4px;
        color: #155724;
      }
    `,
  ],
})
export class Dashboard {
  userProfile: any = null;
  protectedData: any = null;
  users: any[] = [];
  newData: string = '';
  createResult: any = null;

  constructor(
    public authService: AuthCloudService,
    private apiService: ApiService,
    private router: Router
  ) {}

  async ngOnInit() {
    await this.loadUserProfile();
    await this.loadProtectedData();
  }

  async loadUserProfile() {
    try {
      this.userProfile = await this.apiService.getUserProfile();
    } catch (error) {
      console.error('Error loading profile:', error);
    }
  }

  async loadProtectedData() {
    try {
      this.protectedData = await this.apiService.getProtectedData();
    } catch (error) {
      console.error('Error loading data:', error);
    }
  }

  async createData() {
    if (!this.newData.trim()) return;

    try {
      this.createResult = await this.apiService.createProtectedData(this.newData);
      this.newData = '';
    } catch (error) {
      console.error('Error creating data:', error);
    }
  }

  async loadUsers() {
    try {
      this.users = await this.apiService.getUsers();
    } catch (error) {
      console.error('Error loading users:', error);
    }
  }

  logout() {
    this.authService.logout();
  }

  isAdmin(): boolean {
    // Check if user has admin role
    if (this.userProfile?.Claims) {
      return this.userProfile.Claims.some(
        (claim: any) => claim.Type === 'https://schemas.auth0.com/roles' && claim.Value === 'admin'
      );
    }
    return false;
  }
}
