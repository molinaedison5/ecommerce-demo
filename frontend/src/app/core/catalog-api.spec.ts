import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { PLATFORM_ID, TransferState, makeStateKey } from '@angular/core';
import { TestBed } from '@angular/core/testing';

import { API_BASE_URL } from './api-base-url';
import { CatalogApi, PagedProducts } from './catalog-api';

describe('CatalogApi', () => {
  const seeded: PagedProducts = {
    items: [
      {
        id: '6f1c1a10-1f4a-4c0e-9b3a-111111111111',
        name: 'Alimento premium para perros adultos',
        description: 'Croquetas',
        price: 24.9,
        currency: 'USD',
        sku: 'DOG-FOOD-001',
      },
    ],
    page: 1,
    pageSize: 20,
    totalCount: 1,
  };

  function setup(): { api: CatalogApi; http: HttpTestingController; transfer: TransferState } {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: API_BASE_URL, useValue: 'http://localhost:8080' },
        { provide: PLATFORM_ID, useValue: 'browser' },
      ],
    });

    return {
      api: TestBed.inject(CatalogApi),
      http: TestBed.inject(HttpTestingController),
      transfer: TestBed.inject(TransferState),
    };
  }

  it('calls GET /api/catalog/products from the OpenAPI contract', () => {
    const { api, http } = setup();

    api.listProducts(2, 10).subscribe();

    const request = http.expectOne('http://localhost:8080/api/catalog/products?page=2&pageSize=10');
    expect(request.request.method).toBe('GET');
    request.flush({ items: [], page: 2, pageSize: 10, totalCount: 0 });
    http.verify();
  });

  it('reuses the SSR payload instead of calling the API again', () => {
    const { api, http, transfer } = setup();
    const key = makeStateKey<PagedProducts>('catalog.products.1.20');
    transfer.set(key, seeded);

    let result: PagedProducts | undefined;
    api.listProducts().subscribe((page) => {
      result = page;
    });

    http.expectNone(() => true);
    expect(result?.items[0]?.sku).toBe('DOG-FOOD-001');
  });
});
