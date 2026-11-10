import { CurrencyPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Observable, catchError, map, of, startWith } from 'rxjs';

import { CatalogApi, PagedProducts } from '../../core/catalog-api';

type CatalogView =
  | { status: 'loading' }
  | { status: 'error' }
  | { status: 'ready'; page: PagedProducts };

@Component({
  selector: 'app-catalog-page',
  imports: [MatCardModule, MatProgressSpinnerModule, CurrencyPipe],
  templateUrl: './catalog-page.html',
  styleUrl: './catalog-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CatalogPage {
  private readonly catalog = inject(CatalogApi);

  protected readonly view = toSignal(
    this.catalog.listProducts().pipe(
      map((page): CatalogView => ({ status: 'ready', page })),
      catchError((): Observable<CatalogView> => of({ status: 'error' })),
      startWith<CatalogView>({ status: 'loading' }),
    ),
    { initialValue: { status: 'loading' } satisfies CatalogView },
  );

  protected readonly items = computed(() => {
    const view = this.view();
    return view.status === 'ready' ? view.page.items : [];
  });

  protected readonly total = computed(() => {
    const view = this.view();
    return view.status === 'ready' ? view.page.totalCount : 0;
  });
}
