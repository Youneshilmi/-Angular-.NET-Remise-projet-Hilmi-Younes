import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Order } from './models';

const API_URL = 'http://localhost:5148/api/orders';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);

  checkout(bookingDate: string, notes: string) {
    return this.http.post<{ id: number }>(`${API_URL}/checkout`, { bookingDate, notes });
  }

  getMine() {
    return this.http.get<Order[]>(`${API_URL}/mine`);
  }

  getAll() {
    return this.http.get<Order[]>(`${API_URL}/admin`);
  }

  updateStatus(id: number, status: Order['status']) {
    return this.http.patch<void>(`${API_URL}/${id}/status`, { status });
  }
}
