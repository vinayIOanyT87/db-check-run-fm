import { Injectable, ApplicationRef, NgZone } from '@angular/core';
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
/** TransactionService
 * Calls the web api for common Transaction actions
 */
export class TransactionService {
  private currentAuthToken: string;
  private serverUrl = '';
  private grabAuthTokenSubscription: Subscription = null;
  private sendTransactionIntervalID: number = null;
  private hasCleanedUp = false;
  constructor(private http: HttpClient,
      private siteService: SiteService,
      private localStorage: LocalStorageService,
      private serverConfigProvider: ServerConfigService,
      private ngZone: NgZone) {
      this.serverUrl = serverConfigProvider.getServerUrl();
      this.grabAuthTokenSubscription = siteService.authorization
          .pipe(filter(x => (x != null)))
          .pipe(filter(x => (x.SecurityProperties != null)))
          .subscribe(x => this.currentAuthToken = x.SecurityProperties.Token);
      this.startSendingTransactions();
  }

  /**
   * Will return a LoginReponse indicating if it was successfull or not
   * @param user
   * @param password
   * @param site
   */
  getTransactionAliasDetails(transactionGuid: string): Promise<TransactionDetailsDTO> {
      const response = this.http
          .get<TransactionDetailsDTO>(
              this.serverUrl + `/Transaction/TransactionDetails/${transactionGuid}`,
              {
                  headers: new HttpHeaders().set('userToken', this.currentAuthToken)
          });
      return response.toPromise();
  }

  /**
   * saves the current transaction alias field positions
   * @param aliasGuid
   * @param positions key and alias field guids
   */
  saveTransactionAliasPositions(toStore: TransactionAliasStoredFieldsEntry): Promise<void> {
      const response = this.http.post<void>(
          this.serverUrl + `/Transaction/TransactionPlacementInformation`,
          {
              'TransactionAliasGuid': toStore.aliasGuid,
              'PlacementInformation': JSON.stringify(toStore)
          },
          {
              headers: new HttpHeaders().set('userToken', this.currentAuthToken)
          });
      return response.toPromise();
  }

  /**
   * returns the current saved transaction alias field positions, or null if there is not one
   * @param aliasGuid
   */
  getTransactionAliasPositions(aliasGuid: string): Promise<TransactionAliasStoredFieldsEntry> {
      const fieldListResponse = this.http.get<any>(
          this.serverUrl + `/Transaction/TransactionPlacementInformation/${aliasGuid}`,
          {
              headers: new HttpHeaders().set('userToken', this.currentAuthToken)
          })
          .pipe(map(x => {
              const results: any = JSON.parse(x.PlacementInformation);
              // ? this._transactionFieldLists.next(results); ?
              return results;
          }));

      return fieldListResponse.toPromise();
  }

  getTransactionFieldListInfo(transactionGuid: string): Promise<FieldWithAssociatedList[]> {
      const fieldListResponse = this.http.get<FieldWithAssociatedList[]>(
          this.serverUrl + `/Transaction/TransactionDetailAssociatedList/${transactionGuid}`,
          {
              headers: new HttpHeaders().set('userToken', this.currentAuthToken)
          })
          .pipe(map(x => {
              return x;
          }));

      return fieldListResponse.toPromise();
  }

  saveNewTransactionToBackground(transactionToSave: any, transactionAliasGuid: string): void {
      let currentTransactions = <TransactionStoredEntry[]>this.localStorage.get('CurrentStoredTransactions');
      if ((currentTransactions === null || currentTransactions == undefined)) {
          currentTransactions = [];
      }

      currentTransactions.push({
          sentEntry: transactionToSave,
          sentTransactionAliasGuid: transactionAliasGuid,
          returnedEntry: null,
          isSent: false,
          timeStarted: null,
          timeComplete: null,
          error: null
      });

      this.localStorage.store('CurrentStoredTransactions', currentTransactions);
  }

  saveNewTransactionToServer(transactionToSave: TransactionStoredEntry): Promise<TransactionStoredEntry> {
      transactionToSave.isSent = true;
      transactionToSave.timeStarted = new Date();
      return of(transactionToSave)
          .pipe(map(x => {
              x.isSent = true;
              x.timeStarted = new Date();
              return x;
          }))
          .pipe(mergeMap(
              x => {
                  return this.http.post<any>(
                      this.serverUrl + `/Transaction/`,
                      <TransactionInSimplifiedFormat>{
                        'TransactionPropertyValuePairs': transactionToSave.sentEntry,
                        'TransactionAliasGuid': transactionToSave.sentTransactionAliasGuid
                      },
                      {
                          headers: new HttpHeaders().set('userToken', this.currentAuthToken),

                      });
              }
              , 1))
          .pipe(catchError(x => {
              return of(x);
          }))
          .pipe(map(x => {
              transactionToSave.returnedEntry = x;
              transactionToSave.timeComplete = new Date();
              transactionToSave.isSent = true;
              return transactionToSave;
          })).toPromise();
  }

  async startSendingTransactions(): Promise<void> {
    this.ngZone.runOutsideAngular(async () => {
        while (true) {
          if (!(this.localStorage.get('CurrentStoredTransactions') || this.localStorage.get('CurrentStoredTransactions') === undefined)) {
            const currentTransactions = <TransactionStoredEntry[]>this.localStorage.get('CurrentStoredTransactions');
            const toSendIndex = currentTransactions.findIndex(storedTransactions => storedTransactions.isSent === false);
            const toSend = currentTransactions[toSendIndex];

            if (!(toSend === null || toSend === undefined)) {
              const savedTransaction = await this.saveNewTransactionToServer(toSend);
              console.log(savedTransaction);
              currentTransactions.push(savedTransaction);
              currentTransactions.splice(toSendIndex, 1);
              this.localStorage.store('CurrentStoredTransactions', currentTransactions);
            } else if (!this.hasCleanedUp) {
              // since there is nothing to send, lets clean up if this is the first time
              this.hasCleanedUp = true;
              const newTransactions: TransactionStoredEntry[] = [];
              // https://stackoverflow.com/questions/1296358/subtract-days-from-a-date-in-javascript
              const yesterday = new Date(new Date().setDate(new Date().getDate() - 1));
              currentTransactions.forEach(element => {
                const elementCompletedDate = new Date(element.timeComplete);
                if (element.timeComplete != null &&
                  elementCompletedDate > yesterday) {
                  newTransactions.push(element);
                }
              });
              this.localStorage.store('CurrentStoredTransactions', newTransactions);
            }
          }
          // wait a second and then start again
          await new Promise((resolve) => setTimeout(resolve, 1000));
        }
      });
  }

  deleteExistingTransaction(existingTransactionGuid: string): Promise<void> {
    const response = this.http.delete<void>(
      this.serverUrl + `/Transaction/${existingTransactionGuid}`,
      {
          headers: new HttpHeaders().set('userToken', this.currentAuthToken)
      });
    return response.toPromise();
  }

  reverseExistingTransaction(existingTransactionGuid: string): Promise<void> {
    const response = this.http.post<void>(
      this.serverUrl + `/Transaction/${existingTransactionGuid}/Reverse`,
      null,
      {
          headers: new HttpHeaders().set('userToken', this.currentAuthToken)
      });
    return response.toPromise();
  }

  getExistingTransaction(existingTransactionGuid: string): Promise<TransactionInSimplifiedFormat> {
    const response = this.http.get<TransactionInSimplifiedFormat>(
      this.serverUrl + `/Transaction/` + existingTransactionGuid,
      {
          headers: new HttpHeaders().set('userToken', this.currentAuthToken)
      });
    return response.toPromise();
  }

  updateExistingTransaction(fieldsToUpdate: any, transactionAliasGuid: string, existingTransactionGuid: string): Promise<void> {
      return this.http.post<any>(
          this.serverUrl + `/Transaction/` + existingTransactionGuid,
          <TransactionInSimplifiedFormat>{
              'TransactionPropertyValuePairs': fieldsToUpdate,
              'TransactionAliasGuid': transactionAliasGuid
          },
          {
              headers: new HttpHeaders().set('userToken', this.currentAuthToken),

          }).toPromise();
  }

  reverseUpdateTransaction(fieldsToUpdate: any, transactionAliasGuid: string, existingTransactionGuid: string): Promise<void> {
    return this.http.post<any>(
        `${this.serverUrl}/Transaction/${existingTransactionGuid}/ReverseUpdate`,
        fieldsToUpdate,
        {
            headers: new HttpHeaders().set('userToken', this.currentAuthToken),

        }).toPromise();
}
}
