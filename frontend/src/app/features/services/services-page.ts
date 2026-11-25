import { ChangeDetectionStrategy, Component, inject } from '@angular/core';

import { siteContent } from '../../content/site-content';
import { SeoService } from '../../core/seo';
import { ServiceCard } from '../../shared/service-card';

@Component({
  selector: 'app-services-page',
  imports: [ServiceCard],
  templateUrl: './services-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServicesPage {
  protected readonly site = siteContent;

  constructor() {
    inject(SeoService).apply(siteContent.seo.services);
  }
}
