import { TestBed } from '@angular/core/testing';

import { siteContent } from '../content/site-content';
import { SeoService } from './seo';

describe('SeoService', () => {
  it('sets the title, description, canonical URL and local-business JSON-LD', () => {
    TestBed.configureTestingModule({});
    const seo = TestBed.inject(SeoService);

    seo.apply(siteContent.seo.home);
    seo.apply(siteContent.seo.services);

    expect(document.title).toBe(siteContent.seo.services.title);
    expect(document.head.querySelector('meta[name="description"]')?.getAttribute('content')).toBe(
      siteContent.seo.services.description,
    );
    expect(document.head.querySelector('link[rel="canonical"]')?.getAttribute('href')).toContain(
      '/servicios',
    );
    expect(document.head.querySelector('meta[property="og:locale"]')?.getAttribute('content')).toBe(
      'es_EC',
    );

    const scripts = document.head.querySelectorAll('#k9-jsonld');
    expect(scripts).toHaveLength(1);
    const jsonLd = scripts[0]?.textContent ?? '';
    expect(jsonLd).toContain('LocalBusiness');
    expect(jsonLd).toContain('PetStore');
    expect(jsonLd).toContain('+593984204476');
    expect(jsonLd).toContain('Latacunga');
  });
});
