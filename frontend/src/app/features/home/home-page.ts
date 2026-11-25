import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { siteContent } from '../../content/site-content';
import { SeoService } from '../../core/seo';
import { whatsAppUrl } from '../../core/whatsapp';
import { GalleryPairView } from '../../shared/gallery-pair';
import { HoursPanel, SiteMap } from '../../shared/site-facts';
import { ServiceCard } from '../../shared/service-card';

@Component({
  selector: 'app-home-page',
  imports: [RouterLink, ServiceCard, GalleryPairView, HoursPanel, SiteMap],
  templateUrl: './home-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePage {
  protected readonly site = siteContent;
  protected readonly preview = siteContent.gallery.slice(0, 2);
  protected readonly whatsappHref = whatsAppUrl(siteContent.whatsapp.freeClassMessage);

  constructor() {
    inject(SeoService).apply(siteContent.seo.home);
  }
}
