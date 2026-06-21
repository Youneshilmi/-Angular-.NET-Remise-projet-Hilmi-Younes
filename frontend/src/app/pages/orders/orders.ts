import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Order } from '../../core/models';
import { OrderService } from '../../core/order.service';

@Component({ selector: 'app-orders', imports: [RouterLink], templateUrl: './orders.html' })
export class OrdersPage {
  private readonly orderService = inject(OrderService);
  readonly orders = signal<Order[]>([]);
  readonly loading = signal(true);

  constructor() {
    this.orderService.getMine().subscribe({
      next: orders => { this.orders.set(orders); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }
}
