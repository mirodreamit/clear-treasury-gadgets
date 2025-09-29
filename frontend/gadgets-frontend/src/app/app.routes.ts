import { Routes } from '@angular/router';
import { GridComponent } from './grid/grid.component';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { AuthGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: '', component: GridComponent, canActivate: [AuthGuard] }, // protected
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: '**', redirectTo: '' }
];
