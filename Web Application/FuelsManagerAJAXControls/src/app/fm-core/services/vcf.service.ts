import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { SiteService } from './site.service';
import { filter } from 'rxjs/operators';
import { ServerConfigService } from './server-config.service';

@Injectable({
  providedIn: 'root'
})
export class VCFService {
  private currentAuthToken: string;
  private serverUrl = '';

  constructor(private http: HttpClient,
    private siteService: SiteService,
    private serverConfigProvider: ServerConfigService) {
      this.serverUrl = serverConfigProvider.getServerUrl();
      const a = siteService.authorization
          .pipe(filter(x => (x != null)))
          .pipe(filter(x => (x.SecurityProperties != null)))
          .subscribe(x => this.currentAuthToken = x.SecurityProperties.Token);
     }
  getVCF(productId: string, temp: number, density: number): Observable<number> {
    const response = this.http
        .get<number>(
            this.serverUrl + `/Transaction/VCFCalculator/${productId}`,
            {
                headers: new HttpHeaders().set('userToken', this.currentAuthToken),
                params: { temperature: <any>temp, density: <any>density }
            });
    return response;
}
}
