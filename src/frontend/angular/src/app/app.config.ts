import {
  ApplicationConfig,
  importProvidersFrom,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { AuthModule } from '@auth0/auth0-angular';
import { provideHttpClient } from '@angular/common/http';
import { environment } from '../environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideHttpClient(),
    provideRouter(routes),
    importProvidersFrom(
      AuthModule.forRoot({
        domain: 'dev-t5gd-xu5.us.auth0.com', // e.g., dev-xxxx.us.auth0.com
        clientId: 'KVZpOl35JRQozfhrqJuAzyu4WnYF8sRz',
        authorizationParams: {
          redirect_uri: window.location.origin + '/callback',
          audience: environment.auth0Audience,
          // audience: 'https://myapp-api', // ← your custom API identifier
          // scope: 'openid profile email',
        },
        // httpInterceptor: {
        //   allowedList: [
        //     {
        //       uri: environment.apiUrl + '/*',
        //       tokenOptions: {
        //         audience: environment.auth0Audience,
        //         scope: 'read:messages',
        //       },
        //     },
        //   ],
        // },
      })
    ),
  ],
};
