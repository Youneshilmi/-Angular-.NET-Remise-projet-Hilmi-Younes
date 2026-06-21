import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../core/cart.service';
import { OrderService } from '../../core/order.service';

@Component({ selector: 'app-cart', imports: [ReactiveFormsModule, RouterLink], templateUrl: './cart.html' })
export class CartPage {
  readonly cart = inject(CartService);
  private readonly orders = inject(OrderService);
  private readonly router = inject(Router);
  readonly error = signal('');
  readonly loading = signal(false);
  readonly minDate = new Date(Date.now() + 86_400_000).toISOString().slice(0, 10);
  readonly form = new FormGroup({
    bookingDate: new FormControl(this.minDate, { nonNullable: true, validators: [Validators.required] }),
    notes: new FormControl('', { nonNullable: true })
  });

  constructor() {
    this.cart.load().subscribe();
  }

  updateQuantity(serviceId: number, event: Event): void {
    const quantity = Number((event.target as HTMLSelectElement).value);
    this.cart.update(serviceId, quantity).subscribe();
  }

  remove(serviceId: number): void {
    this.cart.remove(serviceId).subscribe();
  }

  checkout(): void {
    if (this.form.invalid || this.cart.items().length === 0) return;
    this.loading.set(true);
    const { bookingDate, notes } = this.form.getRawValue();
    this.orders.checkout(bookingDate, notes).subscribe({
      next: () => {
        this.cart.clearLocal();
        this.router.navigate(['/mes-commandes']);
      },
      error: (error: HttpErrorResponse) => {
        this.error.set(error.error?.message ?? 'La réservation n’a pas pu être créée.');
        this.loading.set(false);
      }
    });
  }
}
