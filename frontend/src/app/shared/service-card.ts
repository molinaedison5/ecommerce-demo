import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { SiteService } from '../content/site-content';
import { whatsAppUrl } from '../core/whatsapp';

@Component({
  selector: 'app-service-card',
  imports: [RouterLink],
  templateUrl: './service-card.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServiceCard {
  readonly service = input.required<SiteService>();

  protected link(message: string): string {
    return whatsAppUrl(message);
  }
}
