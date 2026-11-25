import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { InquiriesApi } from '../../core/inquiries-api';
import { FreeClassPage } from './free-class-page';

describe('FreeClassPage', () => {
  const submitted: unknown[] = [];

  beforeEach(async () => {
    submitted.length = 0;
    await TestBed.configureTestingModule({
      imports: [FreeClassPage],
      providers: [
        provideRouter([]),
        {
          provide: InquiriesApi,
          useValue: {
            submit: (command: unknown) => {
              submitted.push(command);
              return of({
                id: '6f1c1a10-1f4a-4c0e-9b3a-555555555555',
                createdAt: '2026-11-25T15:00:00Z',
                ...(command as object),
              });
            },
          },
        },
      ],
    }).compileComponents();
  });

  function fill(root: HTMLElement, id: string, value: string): void {
    const field = root.querySelector<HTMLInputElement | HTMLTextAreaElement>(`#${id}`);
    if (!field) {
      throw new Error(`Missing #${id}`);
    }
    field.value = value;
    field.dispatchEvent(new Event('input'));
  }

  it('asks for the missing fields before calling the API', async () => {
    const fixture = TestBed.createComponent(FreeClassPage);
    await fixture.whenStable();
    const root = fixture.nativeElement as HTMLElement;

    root.querySelector<HTMLButtonElement>('button[type="submit"]')?.click();
    await fixture.whenStable();

    expect(root.textContent).toContain('Escribe el nombre del dueño.');
    expect(submitted).toHaveLength(0);
  });

  it('stores the request and keeps a WhatsApp link with the same story', async () => {
    const fixture = TestBed.createComponent(FreeClassPage);
    await fixture.whenStable();
    const root = fixture.nativeElement as HTMLElement;

    fill(root, 'ownerName', 'Ana Pérez');
    fill(root, 'phone', '0984204476');
    fill(root, 'breed', 'Mestizo');
    fill(root, 'dogAge', '8 meses');
    fill(root, 'behaviorProblem', 'Tira de la correa');
    root.querySelector<HTMLButtonElement>('button[type="submit"]')?.click();
    await fixture.whenStable();

    expect(submitted).toEqual([
      {
        ownerName: 'Ana Pérez',
        phone: '0984204476',
        breed: 'Mestizo',
        dogAge: '8 meses',
        behaviorProblem: 'Tira de la correa',
      },
    ]);
    expect(root.textContent).toContain('Guardamos tu solicitud');
    const link = root.querySelector<HTMLAnchorElement>('a[href*="wa.me"]');
    expect(link?.href).toContain('wa.me/593984204476');
    expect(decodeURIComponent(link?.href ?? '')).toContain('Tira de la correa');
  });
});
