import { isPlausiblePhone } from './phone';

describe('isPlausiblePhone', () => {
  it('accepts a local mobile and the international business number', () => {
    expect(isPlausiblePhone('0984204476')).toBe(true);
    expect(isPlausiblePhone('+593 98 420 4476')).toBe(true);
  });

  it('rejects letters and numbers that are too short', () => {
    expect(isPlausiblePhone('abc')).toBe(false);
    expect(isPlausiblePhone('123')).toBe(false);
    expect(isPlausiblePhone('   ')).toBe(false);
  });
});
