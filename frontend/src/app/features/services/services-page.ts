import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-services-page',
  imports: [MatCardModule],
  templateUrl: './services-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ServicesPage {
  protected readonly offerings = [
    {
      kind: 'Peluquería',
      detail: 'Baño, corte y cepillado. La reserva todavía no está conectada.',
    },
    {
      kind: 'Visita veterinaria',
      detail: 'Consulta general. El calendario de citas llega en un módulo posterior.',
    },
    {
      kind: 'Guardería',
      detail: 'Cuidado por horas o por día. El carrito de reservas todavía es un cascarón.',
    },
  ];
}
