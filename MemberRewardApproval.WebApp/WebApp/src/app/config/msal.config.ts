import { Configuration } from '@azure/msal-browser';
import { environment } from '../../environments/environment';

export const msalConfig: Configuration = {
  auth: {
    clientId: environment.azure.spaClientId,
    authority: `${environment.azure.authorityBase}/${environment.azure.tenantId}`,
    redirectUri: environment.azure.redirectUri,
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: true,
  },
};
