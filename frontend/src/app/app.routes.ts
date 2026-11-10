import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'catalog' },
  {
    path: 'catalog',
    loadComponent: () => import('./features/catalog/catalog-page').then((m) => m.CatalogPage),
  },
  {
    path: 'services',
    loadComponent: () => import('./features/services/services-page').then((m) => m.ServicesPage),
  },
  {
    path: 'cart',
    loadComponent: () => import('./features/cart/cart-page').then((m) => m.CartPage),
  },
  {
    path: 'account',
    loadComponent: () => import('./features/account/account-page').then((m) => m.AccountPage),
  },
];
