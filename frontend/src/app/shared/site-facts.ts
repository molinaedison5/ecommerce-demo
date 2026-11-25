import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

import { siteContent } from '../content/site-content';

@Component({
  selector: 'app-hours-panel',
  templateUrl: './hours-panel.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HoursPanel {
  protected readonly site = siteContent;
}

@Component({
  selector: 'app-site-map',
  templateUrl: './site-map.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SiteMap {
  protected readonly site = siteContent;
  protected readonly mapUrl = inject(DomSanitizer).bypassSecurityTrustResourceUrl(
    siteContent.map.embedUrl,
  );
}
