import { Injectable } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MsalWrapperService {
  private readonly scopes = environment.azure.scopes;
  private loginInProgress = false; // track login state

  constructor(private msalService: MsalService) {}

  getAccount() {
    const accounts = this.msalService.instance.getAllAccounts();
    return accounts.length ? accounts[0] : null;
  }

  async login() {
    if (this.loginInProgress) {
      console.warn('Login already in progress, skipping...');
      return;
    }

    this.loginInProgress = true;
    try {
      await firstValueFrom(this.msalService.loginPopup({ scopes: this.scopes }));
    } finally {
      this.loginInProgress = false;
    }
  }

  async acquireToken(): Promise<string> {
    const account = this.getAccount();
    if (!account) throw new Error('User not logged in');

    try {
      const result = await this.msalService.instance.acquireTokenSilent({
        account,
        scopes: this.scopes,
      });
      return result.accessToken;
    } catch {
      const result = await firstValueFrom(
        this.msalService.acquireTokenPopup({ scopes: this.scopes })
      );
      return result.accessToken;
    }
  }
}
