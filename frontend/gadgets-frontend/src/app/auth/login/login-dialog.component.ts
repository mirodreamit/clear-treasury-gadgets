import { Component } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './login-dialog.component.html',
  styleUrls: ['./login-dialog.component.scss']
})
export class LoginDialogComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    private dialogRef: MatDialogRef<LoginDialogComponent>
  ) {
    this.form = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onLogin() {
    if (this.form.invalid) return;

    const { email, password } = this.form.value;
    this.auth.login(email!, password!).subscribe({
      next: () => this.dialogRef.close(),
      error: err => console.error(err)
    });
  }

  onSignup() {
    if (this.form.invalid) return;

    const { email, password } = this.form.value;
    this.auth.signup(email!, password!).subscribe({
      next: () => {
        this.dialogRef.close();
        // Optionally navigate to login or homepage
        this.router.navigate(['/']);
      },
      error: err => console.error(err)
    });
  }
}
