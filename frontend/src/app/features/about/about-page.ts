import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { businessPhone, siteContent } from '../../content/site-content';
import { SeoService } from '../../core/seo';
import { whatsAppUrl } from '../../core/whatsapp';
import { HoursPanel, SiteMap } from '../../shared/site-facts';

@Component({
  selector: 'app-about-page',
  imports: [RouterLink, HoursPanel, SiteMap],
  templateUrl: './about-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AboutPage {
  protected readonly site = siteContent;
  protected readonly phone = businessPhone;
  protected readonly whatsappHref = whatsAppUrl(siteContent.whatsapp.defaultMessage);

  constructor() {
    inject(SeoService).apply(siteContent.seo.about);
  }
}
