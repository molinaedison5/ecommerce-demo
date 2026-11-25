import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { HomePage } from './home-page';

describe('HomePage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomePage],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('shows the free-class hook, the slogan and an example service price', async () => {
    const fixture = TestBed.createComponent(HomePage);
    await fixture.whenStable();
    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';

    expect(text).toContain('Agenda tu clase GRATIS');
    expect(text).toContain('Perros felices, familias tranquilas');
    expect(text).toContain('Guardería canina');
    expect(text).toContain('desde $12');
    expect(text).toContain('Vacunas al día');
    expect(text).toContain('Familia de ejemplo');
  });
});
