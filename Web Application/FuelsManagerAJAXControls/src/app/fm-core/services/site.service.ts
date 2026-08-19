import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ServerConfigService } from './server-config.service';
import { LoginResponse } from '../DTO/login-response';
import { KeepPingingService } from './keep-pinging.service';

@Injectable({
  providedIn: 'root'
})
export class SiteService {
  private _authorization: BehaviorSubject<LoginResponse> = new BehaviorSubject<LoginResponse>(null);
  public authorization: Observable<LoginResponse> = this._authorization.asObservable();
  public keepTokenAlive: Subscription;

  constructor(
    private http: HttpClient,
    private serverConfigProvider: ServerConfigService,
    private keepPingingService: KeepPingingService
  ) {}

  /**
   * Will return a LoginReponse indicating if it was successfull or not
   * @param user
   * @param password
   * @param site
   */
  login(
    user: string,
    password: string,
    site: string
  ): Observable<LoginResponse> {
    const serverUrl = this.serverConfigProvider.getServerUrl();
    const response = this.http
      .post<LoginResponse>(serverUrl + '/Site/Login', {
        username: user,
        password,
        site
      })
      .pipe(
        map(x => {
          this._authorization.next(x);
          return x;
        })
      )
      .pipe(
        tap(x => {
          if (x.LoginSuccess) {
            this.keepTokenAlive = this.keepPingingService.startPinging(x.SecurityProperties.Token).subscribe();
          }
        })
      )
      .pipe(
        catchError((err, caught) => {
          throw new Error(err);
        })
      );
    return response;
  }

  /**
   * for use when user is already logged in and we want to use an existing token
   * @param token
   */
  setAuthenicationToken(token: string): Observable<LoginResponse> {
    const serverUrl = this.serverConfigProvider.getServerUrl();
    const response = this.http
      .post<LoginResponse>(
        serverUrl + '/Site/GetLoginResponseAlreadyAuthenticated',
        '"' + token + '"',
        {
          headers: new HttpHeaders()
            .set('userToken', token)
            .set('Content-Type', 'application/json')
        }
      )
      .pipe(
        map(x => {
          this._authorization.next(x);
          return x;
        })
      )
      .pipe(
        tap(x => {
          if (x.LoginSuccess) {
            this.keepTokenAlive = this.keepPingingService.startPinging(x.SecurityProperties.Token).subscribe();
          }
        })
      )
      .pipe(
        catchError((err, caught) => {
          throw new Error(err);
        })
      );
    return response;
  }
}
