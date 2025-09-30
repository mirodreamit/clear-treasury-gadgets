// auth.interceptor.ts
import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';

const isRefreshInProgress = new BehaviorSubject<boolean>(false);
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<any> => {
  const authService = inject(AuthService);

  // Skip Authorization header for refresh endpoint
  const isRefreshEndpoint = req.url.endsWith('/auth/refresh');

  let authReq = req;
  if (!isRefreshEndpoint) {
    const token = authService.getAccessToken();
    if (token) {
      authReq = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });
    }
  }

  return next(authReq).pipe(
    catchError((err: any) => {
      if (err instanceof HttpErrorResponse && err.status === 401 && !isRefreshEndpoint) {
        return handle401Error(authReq, next, authService);
      }
      return throwError(() => err);
    })
  );
};

function handle401Error(req: HttpRequest<any>, next: HttpHandlerFn, authService: AuthService): Observable<any> {
  if (!isRefreshInProgress.value) {
    isRefreshInProgress.next(true);
    refreshTokenSubject.next(null);

    return authService.refreshToken().pipe(
      switchMap((newToken: string) => {
        isRefreshInProgress.next(false);
        refreshTokenSubject.next(newToken);
        return next(req.clone({
          headers: req.headers.set('Authorization', `Bearer ${newToken}`)
        }));
      }),
      catchError(err => {
        isRefreshInProgress.next(false);
        authService.logout();
        return throwError(() => err);
      })
    );
  } else {
    // Wait until token is refreshed
    return refreshTokenSubject.pipe(
      filter(token => token != null),
      take(1),
      switchMap(token => next(req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token!}`)
      })))
    );
  }
}
