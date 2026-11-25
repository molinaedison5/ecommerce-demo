import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { App } from './app';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('renders the K-9 navigation and the WhatsApp shortcut', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const root = fixture.nativeElement as HTMLElement;
    const text = root.textContent ?? '';
    const whatsapp = root.querySelector<HTMLAnchorElement>('a.k9-whatsapp');

    expect(text).toContain('Centro K-9');
    expect(text).toContain('Servicios');
    expect(text).toContain('Clase gratis');
    expect(text).toContain('contenido de ejemplo');
    expect(whatsapp?.href).toContain('https://wa.me/593984204476');
  });
});
