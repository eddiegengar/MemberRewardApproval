import {
  ApplicationConfig,
  provideZoneChangeDetection,
  provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { routes } from './app.routes';

import {
  MsalService,
  MsalBroadcastService,
  MsalGuard,
  MsalInterceptor,
  MSAL_INSTANCE,
} from '@azure/msal-angular';
import { IPublicClientApplication, PublicClientApplication } from '@azure/msal-browser';
import { msalConfig } from './config/msal.config';

// factory with initialize()
export function MSALInstanceFactory(): IPublicClientApplication {
  const instance = new PublicClientApplication(msalConfig);
  // important: initialize before returning
  instance.initialize();
  return instance;
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),

    { provide: MSAL_INSTANCE, useFactory: MSALInstanceFactory },
    MsalService,
    MsalBroadcastService,
    MsalGuard,
    MsalInterceptor,
  ],
};
