import { ChangeDetectionStrategy, Component, inject } from '@angular/core';

import { siteContent } from '../../content/site-content';
import { SeoService } from '../../core/seo';

@Component({
  selector: 'app-cart-page',
  templateUrl: './cart-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CartPage {
  constructor() {
    inject(SeoService).apply(siteContent.seo.cart);
  }
}
