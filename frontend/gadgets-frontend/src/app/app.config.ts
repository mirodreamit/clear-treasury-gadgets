import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { CoreModule } from './core/core.module';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(CoreModule),  // CoreModule providers (AuthInterceptor)
    provideHttpClient(),              // HttpClient for services
    provideRouter(routes)             // Your app routes
  ]
};
