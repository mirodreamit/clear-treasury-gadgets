import { Component } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { AuthService } from './core/auth.service';
import { LoginDialogComponent } from './auth/login/login-dialog.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatButtonModule,
    MatDialogModule
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(
    public authService: AuthService,
    private dialog: MatDialog,
    private router: Router
  ) {}

  openLogin() {
    this.dialog.open(LoginDialogComponent, {
      width: '400px'
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/']); // optional: navigate to home after logout
  }

  navigateOrLogin(path: string) {
    if (this.authService.isLoggedIn()) {
      this.router.navigate([path]);
    } else {
      this.openLogin();
    }
  }
}
