import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-cart-page',
  templateUrl: './cart-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CartPage {}
