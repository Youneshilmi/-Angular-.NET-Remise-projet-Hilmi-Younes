import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/guards';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/catalog/catalog').then(m => m.CatalogPage) },
  { path: 'connexion', loadComponent: () => import('./pages/login/login').then(m => m.LoginPage) },
  { path: 'inscription', loadComponent: () => import('./pages/register/register').then(m => m.RegisterPage) },
  { path: 'panier', canActivate: [authGuard], loadComponent: () => import('./pages/cart/cart').then(m => m.CartPage) },
  { path: 'mes-commandes', canActivate: [authGuard], loadComponent: () => import('./pages/orders/orders').then(m => m.OrdersPage) },
  { path: 'administration', canActivate: [adminGuard], loadComponent: () => import('./pages/admin/admin').then(m => m.AdminPage) },
  { path: '**', redirectTo: '' }
];
