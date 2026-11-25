import { ChangeDetectionStrategy, Component, inject } from '@angular/core';

import { siteContent } from '../../content/site-content';
import { SeoService } from '../../core/seo';
import { GalleryPairView } from '../../shared/gallery-pair';

@Component({
  selector: 'app-gallery-page',
  imports: [GalleryPairView],
  templateUrl: './gallery-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GalleryPage {
  protected readonly site = siteContent;

  constructor() {
    inject(SeoService).apply(siteContent.seo.gallery);
  }
}
