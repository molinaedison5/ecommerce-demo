import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-account-page',
  templateUrl: './account-page.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccountPage {}
