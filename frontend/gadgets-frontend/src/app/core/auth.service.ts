import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private accessToken: string | null = null;

  constructor(private http: HttpClient, private router: Router) {}

  logout(): void {
    this.accessToken = null;
    localStorage.removeItem('refreshToken');
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return !!this.accessToken;
  }

  getAccessToken(): string | null {
    return this.accessToken;
  }

  setAccessToken(token: string) {
    this.accessToken = token;
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  setRefreshToken(token: string) {
    localStorage.setItem('refreshToken', token);
  }

  // Login - store access token in memory, refresh token in localStorage
  login(email: string, password: string): Observable<string> {
    return this.http.post<{ Model: { Token: string; RefreshToken: string } }>(
      `${environment.apiBaseUrl}/auth/login`,
      { email, password }
    ).pipe(
      tap(res => {
        this.accessToken = res.Model.Token;           // in-memory
        this.setRefreshToken(res.Model.RefreshToken); // localStorage
      }),
      map(res => res.Model.Token)
    );
  }

  // Signup - same pattern
  signup(email: string, password: string): Observable<string> {
    return this.http.post<{ Model: { Token: string; RefreshToken: string } }>(
      `${environment.apiBaseUrl}/auth/signup`,
      { email, password }
    ).pipe(
      tap(res => {
        this.accessToken = res.Model.Token;
        this.setRefreshToken(res.Model.RefreshToken);
      }),
      map(res => res.Model.Token)
    );
  }

  // Refresh - read refresh token from localStorage, send in Authorization header
  refreshToken(): Observable<string> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) throw new Error('No refresh token available');

    const headers = new HttpHeaders({
      Authorization: `Bearer ${refreshToken}`
    });

    return this.http.post<{ Model: { Token: string } }>(
      `${environment.apiBaseUrl}/auth/refresh`,
      {},
      { headers }
    ).pipe(
      tap(res => this.accessToken = res.Model.Token), // store in memory
      map(res => res.Model.Token)
    );
  }
}
