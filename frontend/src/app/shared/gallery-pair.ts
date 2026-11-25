import { ChangeDetectionStrategy, Component, input } from '@angular/core';

import { GalleryPair } from '../content/site-content';

@Component({
  selector: 'app-gallery-pair',
  templateUrl: './gallery-pair.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GalleryPairView {
  readonly pair = input.required<GalleryPair>();
}
