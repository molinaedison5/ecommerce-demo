import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';

import { whatsAppUrl } from '../core/whatsapp';
import { WhatsappPrompt } from '../core/whatsapp-prompt';

@Component({
  selector: 'app-whatsapp-button',
  templateUrl: './whatsapp-button.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WhatsappButton {
  private readonly prompt = inject(WhatsappPrompt);

  protected readonly href = computed(() => whatsAppUrl(this.prompt.message()));
}
