import { Routes } from '@angular/router';
import { GadgetsGridComponent } from './gadgets/gadgets-grid.component';
import { CategoriesComponent } from './categories/categories.component';
import { LoginDialogComponent } from './auth/login/login-dialog.component';
import { SignupComponent } from './auth/signup/signup.component';
import { AuthGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/gadgets', pathMatch: 'full' }, // optional: redirect home to gadgets
  { path: 'gadgets', component: GadgetsGridComponent, canActivate: [AuthGuard] },
  { path: 'categories', component: CategoriesComponent },
  { path: 'login', component: LoginDialogComponent },
  { path: 'signup', component: SignupComponent },
  { path: '**', redirectTo: '/gadgets' } // fallback
];
