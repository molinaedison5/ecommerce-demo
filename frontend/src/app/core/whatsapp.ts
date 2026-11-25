import { businessPhone } from '../content/site-content';

export interface FreeClassDraft {
  ownerName: string;
  phone: string;
  breed: string;
  dogAge: string;
  behaviorProblem: string;
}

/** Builds a wa.me link from the single business phone in site content. */
export function whatsAppUrl(message: string): string {
  const digits = businessPhone.e164.replace(/\D/g, '');
  return `https://wa.me/${digits}?text=${encodeURIComponent(message)}`;
}

export function freeClassWhatsappMessage(draft: FreeClassDraft): string {
  const lines = ['Hola, quiero agendar la clase GRATIS en el Centro K-9 (Latacunga).'];
  const fields: readonly [string, string][] = [
    ['Nombre', draft.ownerName],
    ['Teléfono', draft.phone],
    ['Raza', draft.breed],
    ['Edad del perro', draft.dogAge],
    ['Problema de conducta', draft.behaviorProblem],
  ];

  for (const [label, value] of fields) {
    const trimmed = value.trim();
    if (trimmed) {
      lines.push(`${label}: ${trimmed}`);
    }
  }

  return lines.join('\n');
}
