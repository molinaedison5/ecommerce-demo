import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { CatalogApi, ProductDto } from '../../core/catalog-api';
import { CatalogPage } from './catalog-page';

describe('CatalogPage', () => {
  async function render(items: ProductDto[]): Promise<ComponentFixture<CatalogPage>> {
    await TestBed.configureTestingModule({
      imports: [CatalogPage],
      providers: [
        {
          provide: CatalogApi,
          useValue: {
            listProducts: () =>
              of({ items, page: 1, pageSize: 20, totalCount: items.length }),
          },
        },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(CatalogPage);
    await fixture.whenStable();
    return fixture;
  }

  it('renders a product from the catalog slice', async () => {
    const fixture = await render([
      {
        id: '1',
        name: 'Arena aglomerante para gatos',
        description: 'Bajo polvo',
        price: 12.5,
        currency: 'USD',
        sku: 'CAT-LITTER-001',
      },
    ]);

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Arena aglomerante para gatos');
    expect(text).toContain('CAT-LITTER-001');
    expect(text).toContain('1 producto activo');
  });

  it('renders the empty state', async () => {
    const fixture = await render([]);

    expect((fixture.nativeElement as HTMLElement).textContent).toContain('No hay productos activos');
  });
});
