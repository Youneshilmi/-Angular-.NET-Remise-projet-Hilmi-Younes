import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { CartService } from '../../core/cart.service';
import { CatalogService } from '../../core/catalog.service';
import { Category, ServiceOffering } from '../../core/models';

@Component({
  selector: 'app-catalog',
  imports: [ReactiveFormsModule],
  templateUrl: './catalog.html',
  styleUrl: './catalog.scss'
})
export class CatalogPage {
  private readonly catalog = inject(CatalogService);
  private readonly cart = inject(CartService);
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly services = signal<ServiceOffering[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly loading = signal(true);
  readonly message = signal('');
  readonly filters = new FormGroup({
    search: new FormControl('', { nonNullable: true }),
    categoryId: new FormControl(0, { nonNullable: true })
  });

  constructor() {
    this.catalog.getCategories().subscribe(categories => this.categories.set(categories));
    this.load();
  }

  load(): void {
    this.loading.set(true);
    const { search, categoryId } = this.filters.getRawValue();
    this.catalog.getAll(search, categoryId || undefined).subscribe({
      next: services => {
        this.services.set(services);
        this.loading.set(false);
      },
      error: () => {
        this.message.set('Le catalogue est momentanément indisponible.');
        this.loading.set(false);
      }
    });
  }

  reset(): void {
    this.filters.reset({ search: '', categoryId: 0 });
    this.load();
  }

  addToCart(service: ServiceOffering): void {
    if (!this.auth.isAuthenticated()) {
      this.router.navigate(['/connexion']);
      return;
    }

    this.cart.add(service.id).subscribe({
      next: () => this.message.set(`${service.name} a été ajouté au panier.`),
      error: (error: HttpErrorResponse) => this.message.set(error.error?.message ?? 'Ajout impossible.')
    });
  }
}
