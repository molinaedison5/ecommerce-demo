import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/home/home-page').then((m) => m.HomePage),
  },
  {
    path: 'servicios',
    loadComponent: () => import('./features/services/services-page').then((m) => m.ServicesPage),
  },
  {
    path: 'servicios/:slug',
    loadComponent: () =>
      import('./features/services/service-detail-page').then((m) => m.ServiceDetailPage),
  },
  {
    path: 'clase-gratis',
    loadComponent: () => import('./features/free-class/free-class-page').then((m) => m.FreeClassPage),
  },
  {
    path: 'galeria',
    loadComponent: () => import('./features/gallery/gallery-page').then((m) => m.GalleryPage),
  },
  {
    path: 'nosotros',
    loadComponent: () => import('./features/about/about-page').then((m) => m.AboutPage),
  },
  { path: 'services', pathMatch: 'full', redirectTo: 'servicios' },
  {
    path: 'catalog',
    loadComponent: () => import('./features/catalog/catalog-page').then((m) => m.CatalogPage),
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
