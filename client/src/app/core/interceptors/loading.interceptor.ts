import {Injectable} from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import {Observable} from 'rxjs';
import {BusyService} from "../services/busy.service";
import {delay, finalize} from "rxjs/operators";

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private busyService: BusyService) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Disable spinner for POST requests and Orders specifically
    if(req.method === 'POST' && req.url.includes('orders')){
      return next.handle(req);
    }

    if(req.method === 'DELETE') {
      return next.handle(req);
    }

    if (req.url.includes('emailexists')) {
      return next.handle(req);
    }
    this.busyService.busy();
    return next.handle(req).pipe(
      delay(500),
      finalize(() =>
        this.busyService.idle()
      )
    );
  }
}
