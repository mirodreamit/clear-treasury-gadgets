import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Skip Authorization header for refresh endpoint
    const isRefreshEndpoint = req.url.endsWith('/auth/refresh');
    let authReq = req;

    if (!isRefreshEndpoint) {
      const token = this.authService.getAccessToken();
      if (token) {
        authReq = this.addTokenHeader(req, token);
      }
    }

    return next.handle(authReq).pipe(
      catchError(err => {
        if (err instanceof HttpErrorResponse && err.status === 401 && !isRefreshEndpoint) {
          return this.handle401Error(authReq, next);
        }
        return throwError(() => err);
      })
    );
  }

  private addTokenHeader(req: HttpRequest<any>, token: string) {
    return req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
  }

  private handle401Error(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((newToken: string) => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(newToken);
          return next.handle(this.addTokenHeader(req, newToken));
        }),
        catchError(err => {
          this.isRefreshing = false;
          this.authService.logout();
          return throwError(() => err);
        })
      );
    } else {
      // Wait until token is refreshed
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(token => next.handle(this.addTokenHeader(req, token!)))
      );
    }
  }
}
