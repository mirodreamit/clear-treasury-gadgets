import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { GadgetsGridComponent } from './app/gadgets/gadgets-grid.component';
import { CategoriesComponent } from './app/categories/categories.component';
import { authInterceptor } from './app/core/auth.interceptor';
import { routes } from './app/app.routes';

bootstrapApplication(AppComponent, {
  providers: [
    provideHttpClient(
      withInterceptors([authInterceptor])
    ),
    provideRouter(routes),
  ]
});
