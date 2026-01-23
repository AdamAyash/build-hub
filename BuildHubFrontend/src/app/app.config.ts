import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';

import { routes } from './app.routes';

import { providePrimeNG } from 'primeng/config';
import { myPreset } from './core/themes/theme';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),

    // PrimeNG setup
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: myPreset,
      },
    }),

    // NgRx global setup
    provideStore(),
    provideEffects(),
    provideStoreDevtools({ maxAge: 50 }),
  ],
};
