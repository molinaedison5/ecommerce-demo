import { ChangeDetectionStrategy, Component, inject } from '@angular/core';

import { siteContent } from '../../content/site-content';
import { SeoService } from '../../core/seo';

@Component({
  selector: 'app-account-page',
  templateUrl: './account-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccountPage {
  constructor() {
    inject(SeoService).apply(siteContent.seo.account);
  }
}
