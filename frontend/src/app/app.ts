import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';

import { businessPhone, exampleNotice, siteContent } from './content/site-content';
import { whatsAppUrl } from './core/whatsapp';
import { WhatsappButton } from './shared/whatsapp-button';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, WhatsappButton],
  templateUrl: './app.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {
  protected readonly site = siteContent;
  protected readonly phone = businessPhone;
  protected readonly exampleNotice = exampleNotice;
  protected readonly menuOpen = signal(false);
  protected readonly footerWhatsapp = whatsAppUrl(siteContent.whatsapp.defaultMessage);

  constructor() {
    inject(Router)
      .events.pipe(
        filter((event) => event instanceof NavigationEnd),
        takeUntilDestroyed(),
      )
      .subscribe(() => this.menuOpen.set(false));
  }

  protected toggleMenu(): void {
    this.menuOpen.update((open) => !open);
  }
}
