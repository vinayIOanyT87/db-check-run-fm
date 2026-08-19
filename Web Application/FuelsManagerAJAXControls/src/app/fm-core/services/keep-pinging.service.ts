import { Injectable } from '@angular/core';
import { Subscription, Observable, interval } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { SiteService } from './site.service';
import { ServerConfigService } from './server-config.service';
import { filter, tap, mergeMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
/**
 * Keep Pinging Service: Will ping every 30 seconds to keep the session from timing out
 */
export class KeepPingingService {
  private currentAuthToken: string;
  private startTime: number;

  constructor(private _http: HttpClient,
      private serverConfigProvider: ServerConfigService) {
  }

  startPinging(authToken: string): Observable<Date> {
      this.startTime = new Date().getTime();
      this.currentAuthToken = authToken;
      return this.keepPinging();
  }

  private keepPinging(): Observable<Date> {
    const serverUrl = this.serverConfigProvider.getServerUrl();
    const currentPingingTimeout = this.serverConfigProvider.getPingTimeout();
    const stopPinging = this.startTime + (60000 * currentPingingTimeout);
    return interval(1000 * 30)
        .pipe(filter(x => {
          // if true, keep going
          return new Date().getTime() < stopPinging;
        }))
        .pipe(mergeMap(x => {
            return this._http.get<Date>(
                serverUrl + `/Site/ping`,
                {
                    headers: new HttpHeaders().set('userToken', this.currentAuthToken)
                });
        }));
  }
}
