import { isPlatformBrowser, isPlatformServer } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Injectable, PLATFORM_ID, TransferState, inject, makeStateKey } from '@angular/core';
import { Observable, of, tap } from 'rxjs';

import { API_BASE_URL } from './api-base-url';

/** Mirrors `ProductDto` in contracts/openapi.json. */
export interface ProductDto {
  id: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  sku: string;
}

/** Mirrors `PagedResultOfProductDto` for `GET /api/catalog/products`. */
export interface PagedProducts {
  items: ProductDto[];
  page: number;
  pageSize: number;
  totalCount: number;
}

@Injectable({ providedIn: 'root' })
export class CatalogApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL).replace(/\/$/, '');
  private readonly transferState = inject(TransferState);
  private readonly platformId = inject(PLATFORM_ID);

  listProducts(page = 1, pageSize = 20): Observable<PagedProducts> {
    const key = makeStateKey<PagedProducts>(`catalog.products.${page}.${pageSize}`);
    if (isPlatformBrowser(this.platformId) && this.transferState.hasKey(key)) {
      const cached = this.transferState.get(key, emptyPage(page, pageSize));
      this.transferState.remove(key);
      return of(cached);
    }

    return this.http
      .get<PagedProducts>(`${this.apiBaseUrl}/api/catalog/products`, {
        params: { page, pageSize },
      })
      .pipe(
        tap((result) => {
          if (isPlatformServer(this.platformId)) {
            this.transferState.set(key, result);
          }
        }),
      );
  }
}

function emptyPage(page: number, pageSize: number): PagedProducts {
  return { items: [], page, pageSize, totalCount: 0 };
}
