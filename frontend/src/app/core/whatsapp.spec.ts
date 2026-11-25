import { businessPhone } from '../content/site-content';
import { freeClassWhatsappMessage, whatsAppUrl } from './whatsapp';

describe('whatsAppUrl', () => {
  it('uses the single business phone', () => {
    expect(businessPhone.e164).toBe('+593984204476');
    expect(whatsAppUrl('Hola K-9')).toBe(
      `https://wa.me/593984204476?text=${encodeURIComponent('Hola K-9')}`,
    );
  });

  it('fills the free-class message with the fields the family typed', () => {
    const message = freeClassWhatsappMessage({
      ownerName: 'Ana',
      phone: '',
      breed: 'Mestizo',
      dogAge: '2 años',
      behaviorProblem: 'Ladra',
    });

    expect(message).toContain('clase GRATIS');
    expect(message).toContain('Nombre: Ana');
    expect(message).toContain('Raza: Mestizo');
    expect(message).not.toContain('Teléfono:');
    expect(message).toContain('Ladra');
  });
});
