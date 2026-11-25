import { DOCUMENT } from '@angular/common';
import { Injectable, inject } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';

import { PageSeo, businessPhone, pageUrl, siteContent, siteOrigin } from '../content/site-content';

@Injectable({ providedIn: 'root' })
export class SeoService {
  private readonly title = inject(Title);
  private readonly meta = inject(Meta);
  private readonly document = inject(DOCUMENT);

  apply(page: PageSeo): void {
    const url = pageUrl(page.path);
    this.title.setTitle(page.title);
    this.meta.updateTag({ name: 'description', content: page.description });
    this.meta.updateTag({ property: 'og:title', content: page.title });
    this.meta.updateTag({ property: 'og:description', content: page.description });
    this.meta.updateTag({ property: 'og:type', content: 'website' });
    this.meta.updateTag({ property: 'og:locale', content: 'es_EC' });
    this.meta.updateTag({ property: 'og:url', content: url });
    this.meta.updateTag({ property: 'og:site_name', content: siteContent.brand.name });
    this.meta.updateTag({ property: 'og:image', content: `${siteOrigin}/media/hero.svg` });
    this.upsertLink('canonical', url);
    this.upsertJsonLd(localBusinessJsonLd(url));
  }

  private upsertLink(rel: string, href: string): void {
    let link = this.document.head.querySelector<HTMLLinkElement>(`link[rel="${rel}"]`);
    if (!link) {
      link = this.document.createElement('link');
      link.rel = rel;
      this.document.head.appendChild(link);
    }
    link.href = href;
  }

  private upsertJsonLd(data: unknown): void {
    let script = this.document.getElementById('k9-jsonld');
    if (!script) {
      script = this.document.createElement('script');
      script.id = 'k9-jsonld';
      script.setAttribute('type', 'application/ld+json');
      this.document.head.appendChild(script);
    }
    script.textContent = JSON.stringify(data);
  }
}

function localBusinessJsonLd(url: string) {
  const { brand, address, hours, services } = siteContent;
  return {
    '@context': 'https://schema.org',
    '@type': ['LocalBusiness', 'PetStore'],
    name: brand.name,
    description: brand.proposal,
    url,
    image: `${siteOrigin}/media/hero.svg`,
    telephone: businessPhone.e164,
    slogan: brand.slogan,
    address: {
      '@type': 'PostalAddress',
      streetAddress: address.line,
      addressLocality: address.locality,
      addressRegion: address.region,
      addressCountry: address.countryCode,
    },
    areaServed: `${address.locality}, ${address.region}`,
    knowsLanguage: 'es',
    openingHoursSpecification: hours
      .filter((slot) => slot.opens && slot.closes)
      .map((slot) => ({
        '@type': 'OpeningHoursSpecification',
        dayOfWeek: slot.schemaDays,
        opens: slot.opens,
        closes: slot.closes,
      })),
    hasOfferCatalog: {
      '@type': 'OfferCatalog',
      name: 'Servicios caninos',
      itemListElement: services.map((service) => ({
        '@type': 'Offer',
        itemOffered: {
          '@type': 'Service',
          name: service.name,
          description: service.summary,
        },
      })),
    },
  };
}
