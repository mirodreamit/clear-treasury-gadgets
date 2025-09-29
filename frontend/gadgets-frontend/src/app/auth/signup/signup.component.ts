import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent {
  form!: FormGroup; // declare without initializing
  loading = signal(false);
  error = signal('');
  success = signal('');

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    // Initialize the form inside the constructor
    this.form = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }

  submit() {
    if (this.form.invalid) return;

    const { email, password, confirmPassword } = this.form.value;
    if (password !== confirmPassword) {
      this.error.set("Passwords don't match");
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.auth.signup(email!, password!).subscribe({
      next: () => {
        this.loading.set(false);
        this.success.set('Signup successful! You can now login.');
        this.router.navigate(['/login']);
      },
      error: err => {
        this.loading.set(false);
        this.error.set('Signup failed.');
        console.error(err);
      }
    });
  }
}
