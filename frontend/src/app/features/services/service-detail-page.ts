import { ChangeDetectionStrategy, Component, computed, effect, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { map } from 'rxjs';

import { serviceBySlug, siteContent } from '../../content/site-content';
import { SeoService } from '../../core/seo';
import { whatsAppUrl } from '../../core/whatsapp';
import { WhatsappPrompt } from '../../core/whatsapp-prompt';

@Component({
  selector: 'app-service-detail-page',
  imports: [RouterLink],
  templateUrl: './service-detail-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServiceDetailPage {
  private readonly seo = inject(SeoService);
  private readonly whatsapp = inject(WhatsappPrompt);

  protected readonly service = toSignal(
    inject(ActivatedRoute).paramMap.pipe(map((params) => serviceBySlug(params.get('slug')) ?? null)),
    { initialValue: serviceBySlug(inject(ActivatedRoute).snapshot.paramMap.get('slug')) ?? null },
  );

  protected readonly whatsappHref = computed(() =>
    whatsAppUrl(this.service()?.whatsappMessage ?? siteContent.whatsapp.defaultMessage),
  );

  constructor() {
    this.publish(this.service());
    effect(() => this.publish(this.service()));
  }

  private publish(service: ReturnType<typeof serviceBySlug> | null): void {
    if (service) {
      this.whatsapp.use(service.whatsappMessage);
      this.seo.apply({
        title: service.seoTitle,
        description: service.seoDescription,
        path: `/servicios/${service.slug}`,
      });
      return;
    }

    this.whatsapp.use(siteContent.whatsapp.defaultMessage);
    this.seo.apply({
      title: 'Servicio no encontrado | Centro K-9',
      description: 'Ese servicio no está en el Centro K-9. Vuelve al listado de servicios en Latacunga.',
      path: '/servicios',
    });
  }
}
