const allowedPhone = /^\+?[0-9][0-9\s().-]{6,31}$/;

/** Mirrors FreeClassRequest.IsPlausiblePhone: 7–15 digits, no letters. */
export function isPlausiblePhone(phone: string | null | undefined): boolean {
  if (!phone || phone.trim().length === 0 || phone.trim().length > 32) {
    return false;
  }

  const trimmed = phone.trim();
  if (!allowedPhone.test(trimmed)) {
    return false;
  }

  const digits = [...trimmed].filter((character) => character >= '0' && character <= '9').length;
  return digits >= 7 && digits <= 15;
}
