import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { ServiceDetailPage } from './service-detail-page';

describe('ServiceDetailPage', () => {
  async function render(slug: string): Promise<HTMLElement> {
    TestBed.configureTestingModule({
      imports: [ServiceDetailPage],
      providers: [
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(convertToParamMap({ slug })),
            snapshot: { paramMap: convertToParamMap({ slug }) },
          },
        },
      ],
    });

    const fixture = TestBed.createComponent(ServiceDetailPage);
    await fixture.whenStable();
    return fixture.nativeElement as HTMLElement;
  }

  it('shows the example price and the boarding requirements', async () => {
    const root = await render('hospedaje-canino');
    const text = root.textContent ?? '';

    expect(text).toContain('Hospedaje canino');
    expect(text).toContain('desde $18');
    expect(text).toContain('Vacunas al día');
    expect(text).toContain('Desparasitación');
    expect(root.querySelector('a[href*="wa.me/593984204476"]')).not.toBeNull();
  });

  it('explains a missing service', async () => {
    const text = (await render('no-existe')).textContent ?? '';

    expect(text).toContain('No encontramos ese servicio');
  });
});
