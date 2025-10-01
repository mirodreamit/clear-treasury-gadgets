import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap, map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';

interface AuthResponse {
  model: {
    token: string;
    refreshToken: string;
    displayName?: string; // optional, depends on backend
  };
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private accessToken: string | null = null;
  private _displayName: string | null = null;

  constructor(private http: HttpClient, private router: Router) {}

  // --- Accessors ---
  get displayName(): string | null {
    return this._displayName;
  }

  getAccessToken(): string | null {
    return this.accessToken;
  }

  // --- Token management ---
  setAccessToken(token: string) {
    this.accessToken = token;
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refreshToken');
  }

  setRefreshToken(token: string) {
    localStorage.setItem('refreshToken', token);
  }

  // --- Auth state ---
  isLoggedIn(): boolean {
    return !!this.accessToken;
  }

  logout(): void {
    this.accessToken = null;
    this._displayName = null;
    localStorage.removeItem('refreshToken');
    this.router.navigate(['/login']);
  }

  // --- API calls ---
  login(email: string, password: string): Observable<string> {
    return this.http.post<AuthResponse>(
      `${environment.apiBaseUrl}/auth/login`,
      { email, password }
    ).pipe(
      tap(res => this.handleAuthResponse(res, email)),
      map(res => res.model.token)
    );
  }

  signup(email: string, password: string): Observable<string> {
    return this.http.post<AuthResponse>(
      `${environment.apiBaseUrl}/auth/signup`,
      { email, password }
    ).pipe(
      tap(res => this.handleAuthResponse(res, email)),
      map(res => res.model.token)
    );
  }

  refreshToken(): Observable<string> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) throw new Error('No refresh token available');

    const headers = new HttpHeaders({
      Authorization: `Bearer ${refreshToken}`
    });

    return this.http.post<AuthResponse>(
      `${environment.apiBaseUrl}/auth/refresh`,
      {},
      { headers }
    ).pipe(
      tap(res => this.handleAuthResponse(res)),
      map(res => res.model.token)
    );
  }

  // --- Helper to DRY up token handling ---
  private handleAuthResponse(res: AuthResponse, fallbackEmail?: string): void {
    this.accessToken = res.model.token;
    this.setRefreshToken(res.model.refreshToken);
    this._displayName = res.model.displayName ?? fallbackEmail ?? null;
  }
}
