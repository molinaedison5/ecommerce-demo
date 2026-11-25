import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from './api-base-url';

/** Mirrors SubmitFreeClassRequestCommand / FreeClassRequestDto in contracts/openapi.json. */
export interface FreeClassRequestDto {
  id: string;
  ownerName: string;
  phone: string;
  breed: string;
  dogAge: string;
  behaviorProblem: string;
  createdAt: string;
}

export type SubmitFreeClassRequest = Omit<FreeClassRequestDto, 'id' | 'createdAt'>;

@Injectable({ providedIn: 'root' })
export class InquiriesApi {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL).replace(/\/$/, '');

  submit(command: SubmitFreeClassRequest): Observable<FreeClassRequestDto> {
    return this.http.post<FreeClassRequestDto>(
      `${this.apiBaseUrl}/api/inquiries/free-classes`,
      command,
    );
  }
}
