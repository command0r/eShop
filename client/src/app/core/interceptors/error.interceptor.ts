import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {NavigationExtras, Router} from "@angular/router";
import {catchError, delay} from "rxjs/operators";
import {ToastrService} from "ngx-toastr";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      // @ts-ignore
      catchError(error => {
        if(error) {
          if(error.status === 400) {
            if(error.error.errors) {
              throw error.error;
            } else {
              this.toastr.error(error.error.message, error.error.statusCode);
            }
          }
          if(error.status === 401) {
            this.toastr.error(error.error.message, error.error.statusCode);
          }
          if(error.status === 404) {
            this.router.navigateByUrl('/not-found');
          }
          if(error.status === 500) {
            // Passing state to the route (to make the error message available in the redirected page)
            const navigatioExtras: NavigationExtras = {state: {error: error.error}};
            this.router.navigateByUrl('server-error', navigatioExtras);
          }

          return throwError(error);
        }
      })
    );
  }
}
