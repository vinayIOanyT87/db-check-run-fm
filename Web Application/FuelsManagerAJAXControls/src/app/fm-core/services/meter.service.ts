import { Injectable, ApplicationRef } from '@angular/core';
import { BehaviorSubject, Observable, of, interval, zip, Subscription } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { SiteService } from './site.service';
import { KeepPingingService } from './keep-pinging.service';
import { ServerConfigService } from './server-config.service';
import { filter, tap, map, mergeMap, catchError } from 'rxjs/operators';
import { LocalStorageService } from './local-storage.service';
import { TransactionAliasField, FieldWithAssociatedList, TransactionDetailsDTO } from '../DTO/transaction-alias-placement-information';
import { TransactionAliasStoredFieldsEntry, TransactionStoredEntry } from '../DTO/transaction-alias-stored-fields-entry';
import { TransactionInSimplifiedFormat } from '../DTO/transaction-in-simplified-format';

@Injectable({
  providedIn: 'root'
})
export class MeterService {
  private currentAuthToken: string;
  private serverUrl = '';
  private grabAuthTokenSubscription: Subscription = null;
  constructor(private http: HttpClient,
    private siteService: SiteService,
    private serverConfigProvider: ServerConfigService) {
    this.serverUrl = serverConfigProvider.getServerUrl();
    this.grabAuthTokenSubscription = siteService.authorization
        .pipe(filter(x => (x != null)))
        .pipe(filter(x => (x.SecurityProperties != null)))
        .subscribe(x => this.currentAuthToken = x.SecurityProperties.Token);
  }


  /**
   * Will return a LoginReponse indicating if it was successfull or not
   * @param user
   * @param password
   * @param site
   */
  didMeterRollover(meterId: string, transactionAliasGuid: string, meterStart: number, meterStop: number): Observable<MeterRollOverDTO> {
    const response = this.http
        .get<MeterRollOverDTO>(
            this.serverUrl
            + `/Meter/MeterHasRolled?meterId=${meterId}&transactionAliasGuid=${transactionAliasGuid}`
            + `&meterStart=${meterStart}&meterStop=${meterStop}`,
            {
                headers: new HttpHeaders().set('userToken', this.currentAuthToken)
        });
    return response;
  }
}

export interface MeterRollOverDTO {
  MeterOverflowed: boolean;
  Difference: number;
  NumberOfDigitsInMeter: number;
}
