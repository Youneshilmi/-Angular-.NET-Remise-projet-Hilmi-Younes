import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin, Observable } from 'rxjs';
import { AdminService } from '../../core/admin.service';
import { CatalogService } from '../../core/catalog.service';
import { Category, DashboardSummary, Order, ServiceInput, ServiceOffering } from '../../core/models';
import { OrderService } from '../../core/order.service';

@Component({ selector: 'app-admin', imports: [ReactiveFormsModule], templateUrl: './admin.html' })
export class AdminPage {
  private readonly admin = inject(AdminService);
  private readonly catalog = inject(CatalogService);
  private readonly orderService = inject(OrderService);
  readonly summary = signal<DashboardSummary | null>(null);
  readonly services = signal<ServiceOffering[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly orders = signal<Order[]>([]);
  readonly editingId = signal<number | null>(null);
  readonly message = signal('');
  readonly form = new FormGroup({
    categoryId: new FormControl(0, { nonNullable: true, validators: [Validators.min(1)] }),
    name: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    description: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    durationMinutes: new FormControl(60, { nonNullable: true, validators: [Validators.min(1)] }),
    price: new FormControl(0, { nonNullable: true, validators: [Validators.min(0)] }),
    imageUrl: new FormControl('', { nonNullable: true }),
    isActive: new FormControl(true, { nonNullable: true })
  });

  constructor() { this.load(); }

  load(): void {
    forkJoin({
      summary: this.admin.getDashboard(),
      services: this.catalog.getAllForAdmin(),
      categories: this.catalog.getCategories(),
      orders: this.orderService.getAll()
    }).subscribe(data => {
      this.summary.set(data.summary);
      this.services.set(data.services);
      this.categories.set(data.categories);
      this.orders.set(data.orders);
    });
  }

  edit(service: ServiceOffering): void {
    this.editingId.set(service.id);
    this.form.setValue({
      categoryId: service.categoryId,
      name: service.name,
      description: service.description,
      durationMinutes: service.durationMinutes,
      price: service.price,
      imageUrl: service.imageUrl,
      isActive: service.isActive
    });
    window.scrollTo({ top: 250, behavior: 'smooth' });
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.form.reset({ categoryId: 0, name: '', description: '', durationMinutes: 60, price: 0, imageUrl: '', isActive: true });
  }

  save(): void {
    if (this.form.invalid) return;
    const input = this.form.getRawValue() as ServiceInput;
    const request: Observable<unknown> = this.editingId()
      ? this.catalog.update(this.editingId()!, input)
      : this.catalog.create(input);
    request.subscribe(() => {
      this.message.set(this.editingId() ? 'Service mis à jour.' : 'Service créé.');
      this.cancelEdit();
      this.load();
    });
  }

  deleteService(id: number): void {
    if (!window.confirm('Désactiver ce service ?')) return;
    this.catalog.delete(id).subscribe(() => this.load());
  }

  updateStatus(order: Order, event: Event): void {
    const status = (event.target as HTMLSelectElement).value as Order['status'];
    this.orderService.updateStatus(order.id, status).subscribe(() => this.load());
  }
}
