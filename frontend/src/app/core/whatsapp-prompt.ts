import { Injectable, inject, signal } from '@angular/core';
import { NavigationStart, Router } from '@angular/router';
import { filter } from 'rxjs';

import { siteContent } from '../content/site-content';

/** Message used by the floating WhatsApp button. Pages replace it for a service. */
@Injectable({ providedIn: 'root' })
export class WhatsappPrompt {
  readonly message = signal<string>(siteContent.whatsapp.defaultMessage);

  constructor() {
    inject(Router)
      .events.pipe(filter((event) => event instanceof NavigationStart))
      .subscribe(() => this.message.set(siteContent.whatsapp.defaultMessage));
  }

  use(message: string): void {
    this.message.set(message);
  }
}
