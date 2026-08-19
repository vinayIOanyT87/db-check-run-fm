import { Injectable } from '@angular/core';
import { CurrentTransactionFieldBagService } from './current-transaction-field-bag.service';
import { filter, map, tap } from 'rxjs/operators';
import { LocalStorageService } from '../../fm-core/services/local-storage.service';
import { Observable, combineLatest, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SaveMeterStartOnMeterService {
  private _retrieveMeterStartOnMeterIDSet = false;
  private _currentUser = '';
  private _currentMeterEnd = '';
  private _currentMeterId = '';
  constructor(private _fieldBagService: CurrentTransactionFieldBagService,
    private _localStorage: LocalStorageService) { }

  bootstrap(currentUser: string, retrieveMeterStartOnMeterIDSet: boolean = false): Observable<void> {
    this._retrieveMeterStartOnMeterIDSet = retrieveMeterStartOnMeterIDSet;
    if (!this._retrieveMeterStartOnMeterIDSet) {
      return of(null);
    }
    this._currentUser = currentUser;
    // update meter end on change
    const meterEndSubcription = this._fieldBagService.hasChanged
      .pipe(filter((x) => x.field === 'MeterStop'))
      .pipe(filter(x => !x.initialData))
      .pipe(map((x) => {
        this._currentMeterEnd = x.value;
      }));
    const meterIdSubscription = this._fieldBagService.hasChanged
      .pipe(filter((x) => x.field === 'MeterID'))
      .pipe(filter((x) => !x.initialData))
      .pipe(map((x) => {
        this._currentMeterId = x.value;

        const currentValue = this._localStorage.get<string>(this.getKey());
        if (!(currentValue === null || currentValue === undefined) &&
          currentValue !== '') {
            this._fieldBagService.addOrSetField('MeterStart', currentValue, 'SaveMeterStartOnMeterService');
        }
      }));
      return combineLatest(meterEndSubcription, meterIdSubscription, (x, y) => {});
  }
  getKey(): string {
    return 'User:' + this._currentUser + ';MeterId:' + this._currentMeterId;
  }
  save(): void {
    if (!this._retrieveMeterStartOnMeterIDSet) {
      return;
    }
    const key = this.getKey();
    this._localStorage.store(key, this._currentMeterEnd);
  }
}
