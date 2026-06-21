import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { switchMap, tap } from 'rxjs';
import { CartItem } from './models';

const API_URL = 'http://localhost:5148/api/cart';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly http = inject(HttpClient);
  private readonly itemsState = signal<CartItem[]>([]);

  readonly items = this.itemsState.asReadonly();
  readonly count = computed(() => this.itemsState().reduce((sum, item) => sum + item.quantity, 0));
  readonly total = computed(() => this.itemsState().reduce((sum, item) => sum + item.lineTotal, 0));

  load() {
    return this.http.get<CartItem[]>(API_URL).pipe(tap(items => this.itemsState.set(items)));
  }

  add(serviceId: number, quantity = 1) {
    return this.http.post<void>(API_URL, { serviceId, quantity }).pipe(switchMap(() => this.load()));
  }

  update(serviceId: number, quantity: number) {
    return this.http.put<void>(`${API_URL}/${serviceId}`, { quantity }).pipe(switchMap(() => this.load()));
  }

  remove(serviceId: number) {
    return this.http.delete<void>(`${API_URL}/${serviceId}`).pipe(switchMap(() => this.load()));
  }

  clearLocal(): void {
    this.itemsState.set([]);
  }
}
