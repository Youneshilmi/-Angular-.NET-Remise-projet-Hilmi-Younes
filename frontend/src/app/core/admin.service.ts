import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { DashboardSummary } from './models';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly http = inject(HttpClient);

  getDashboard() {
    return this.http.get<DashboardSummary>('http://localhost:5148/api/admin/dashboard');
  }
}
