import { InjectionToken } from '@angular/core';

/** Origin of the HTTP API. The server build reads `API_BASE_URL`; the browser uses localhost. */
export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL');
