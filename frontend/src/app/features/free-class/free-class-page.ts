import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { siteContent } from '../../content/site-content';
import { InquiriesApi, SubmitFreeClassRequest } from '../../core/inquiries-api';
import { isPlausiblePhone } from '../../core/phone';
import { SeoService } from '../../core/seo';
import { freeClassWhatsappMessage, whatsAppUrl } from '../../core/whatsapp';
import { WhatsappPrompt } from '../../core/whatsapp-prompt';

type SaveState =
  | { status: 'editing' }
  | { status: 'saving' }
  | { status: 'saved'; id: string }
  | { status: 'error'; message: string };

@Component({
  selector: 'app-free-class-page',
  imports: [ReactiveFormsModule],
  templateUrl: './free-class-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FreeClassPage {
  private readonly inquiries = inject(InquiriesApi);

  protected readonly site = siteContent;
  protected readonly state = signal<SaveState>({ status: 'editing' });
  protected readonly whatsappHref = signal(whatsAppUrl(freeClassWhatsappMessage(emptyDraft())));

  protected readonly form = inject(FormBuilder).nonNullable.group({
    ownerName: ['', [Validators.required, Validators.maxLength(120)]],
    phone: ['', [Validators.required, Validators.maxLength(32), phoneValidator]],
    breed: ['', [Validators.required, Validators.maxLength(80)]],
    dogAge: ['', [Validators.required, Validators.maxLength(40)]],
    behaviorProblem: ['', [Validators.required, Validators.maxLength(2000)]],
  });

  constructor() {
    inject(SeoService).apply(siteContent.seo.freeClass);
    inject(WhatsappPrompt).use(siteContent.whatsapp.freeClassMessage);
    this.form.valueChanges.pipe(takeUntilDestroyed()).subscribe((value) => {
      this.whatsappHref.set(whatsAppUrl(freeClassWhatsappMessage({ ...emptyDraft(), ...value })));
    });
  }

  protected invalid(name: keyof SubmitFreeClassRequest): boolean {
    const control = this.form.controls[name];
    return control.invalid && (control.touched || control.dirty);
  }

  protected message(name: keyof SubmitFreeClassRequest): string {
    const errors = this.form.controls[name].errors;
    if (!errors) {
      return '';
    }
    if (errors['required']) {
      return requiredCopy[name];
    }
    if (errors['maxlength']) {
      return 'El texto es demasiado largo.';
    }
    if (errors['phone']) {
      return 'Usa un teléfono de 7 a 15 dígitos. Puedes incluir +, espacios o guiones.';
    }
    return 'Revisa este campo.';
  }

  protected save(): void {
    if (this.state().status === 'saving') {
      return;
    }
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const command = this.form.getRawValue();
    this.state.set({ status: 'saving' });
    this.inquiries.submit(command).subscribe({
      next: (saved) => this.state.set({ status: 'saved', id: saved.id }),
      error: (error: unknown) => {
        this.state.set({ status: 'error', message: messageFor(error) });
        applyServerErrors(this.form, error);
      },
    });
  }

  protected editAgain(): void {
    this.form.reset();
    this.state.set({ status: 'editing' });
  }
}

const requiredCopy: Record<keyof SubmitFreeClassRequest, string> = {
  ownerName: 'Escribe el nombre del dueño.',
  phone: 'Escribe un teléfono.',
  breed: 'Escribe la raza. Si no la sabes, pon mestizo.',
  dogAge: 'Escribe la edad del perro, por ejemplo 8 meses.',
  behaviorProblem: 'Cuéntanos el problema de conducta.',
};

function emptyDraft(): SubmitFreeClassRequest {
  return { ownerName: '', phone: '', breed: '', dogAge: '', behaviorProblem: '' };
}

function phoneValidator(control: AbstractControl) {
  return isPlausiblePhone(String(control.value ?? '')) ? null : { phone: true };
}

function messageFor(error: unknown): string {
  if (error instanceof HttpErrorResponse && error.status === 0) {
    return 'No llegamos a la API. Puedes continuar por WhatsApp.';
  }
  if (error instanceof HttpErrorResponse && error.status === 400) {
    return 'Revisa los datos. Si prefieres, envía lo mismo por WhatsApp.';
  }
  return 'No se pudo guardar la solicitud. Puedes continuar por WhatsApp.';
}

function applyServerErrors(
  form: FreeClassPage['form'],
  error: unknown,
): void {
  if (!(error instanceof HttpErrorResponse) || error.status !== 400) {
    return;
  }
  const errors = (error.error as { errors?: Record<string, string[]> } | null)?.errors;
  if (!errors) {
    return;
  }
  for (const name of Object.keys(requiredCopy) as (keyof SubmitFreeClassRequest)[]) {
    if (errors[name]?.length) {
      form.controls[name].setErrors(name === 'phone' ? { phone: true } : { required: true });
    }
  }
}
