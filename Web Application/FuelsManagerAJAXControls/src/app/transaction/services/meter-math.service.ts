import { Injectable, ApplicationRef } from '@angular/core';
import { BehaviorSubject, combineLatest, ObservableInput, Observable, forkJoin, of } from 'rxjs';
import { CurrentTransactionFieldBagService } from './current-transaction-field-bag.service';
import { distinctUntilChanged, filter, map, tap, combineAll, mergeMap, startWith } from 'rxjs/operators';
import { MeterService } from '../../fm-core/services/meter.service';

@Injectable({
  providedIn: 'root'
})
export class MeterMathService {
  private meterStart = '';
  private meterStop = '';
  private meterGross = '';
  private meterNet = '';
  private meter = '';

  constructor(private _fieldBagService: CurrentTransactionFieldBagService,
    private _meterService: MeterService) {}

  bootstrap(): Observable<{ gross: number, net: number }> {
    const start = this._fieldBagService.hasChanged
      .pipe(filter(x => x.field === 'MeterStart'))
      .pipe(filter(x => x.source !== 'MeterMathService'))
      .pipe(startWith({field: 'MeterStart', value: this._fieldBagService.getField('MeterStart'), initialData: true}))
      // .pipe(filter(x => !x.initialData))
      .pipe(distinctUntilChanged());
    const stop = this._fieldBagService.hasChanged
      .pipe(filter(x => x.field === 'MeterStop'))
      .pipe(filter(x => x.source !== 'MeterMathService'))
      .pipe(startWith({field: 'MeterStop', value: this._fieldBagService.getField('MeterStop'), initialData: true}))
      // .pipe(filter(x => !x.initialData))
      .pipe(distinctUntilChanged());
    const meter = this._fieldBagService.hasChanged
      .pipe(startWith({field: 'MeterID', value: this._fieldBagService.getField('MeterID'), initialData: true}))
      .pipe(filter(x => x.field === 'MeterID'))
      .pipe(distinctUntilChanged())
      .pipe(map(x => {
        this.meter = x.value;
        return x.value;
      }));
    const grossObservable = combineLatest(
      start,
      stop,
      meter,
      (x, y) => ({ start: x, stop: y, meter: meter })
    )
      .pipe(mergeMap((x => {
        // check for nulls, empty strings and initial values
        if (x.start.value === null ||
          x.stop.value === null ||
          x.start.value === '' ||
          x.stop.value === '' ||
          (x.start.initialData &&
          x.stop.initialData)) {
            return of({ gross: null, overflowCheck: null, start: null, stop: null});
          }
        const parsedStart = +x.start.value;
        const parsedStop = +x.stop.value;
        if (isNaN(parsedStart) || isNaN(parsedStop)) {
          return of({ gross: null, overflowCheck: null, start: null, stop: null});
        }
        const newGross = parsedStop - parsedStart;
        // do not check for meter overflow if it is positive
        if (((newGross > 0 || this.meter === null || this.meter === '') &&
          this._fieldBagService.currentSettings.transactionAliasType !== 4)) {
          return of({ gross: newGross, overflowCheck: null, start: parsedStart, stop: parsedStop});
        }
        if ((this._fieldBagService.currentSettings.transactionAliasType === 4 &&
          (this.meter === null || this.meter === '' ))) {
          return of({ gross: newGross, overflowCheck: null, start: parsedStart, stop: parsedStop});
        }
        return this._meterService.didMeterRollover(this.meter, this._fieldBagService.currentSettings.transactionAliasGuid,
          parsedStart, parsedStop)
          .pipe(map(y => {
            return {gross: newGross, overflowCheck: y, start: parsedStart, stop: parsedStop};
          }));
      })))
      .pipe(map(x => {
        if (x.gross != null) {
          let toSet = x.gross;
          if (x.overflowCheck) {
            toSet = x.overflowCheck.Difference;
          }
          this._fieldBagService.addOrSetField('GrossQuantity', toSet.toString(), 'MeterMathService');
        }
        return x.gross;
      }));

    const gross = this._fieldBagService.hasChanged
      .pipe(filter(x => x.field === 'GrossQuantity'))
      .pipe(filter(x => x != null))
      .pipe(startWith({field: 'GrossQuantity', value: this._fieldBagService.getField('GrossQuantity'), initialData: true}))
      .pipe(distinctUntilChanged());
    const vcf = this._fieldBagService.hasChanged
      .pipe(filter(x => x.field === 'Vcf'))
      .pipe(filter(x => x != null))
      .pipe(startWith({field: 'Vcf', value: this._fieldBagService.getField('Vcf'), initialData: true}))
      .pipe(distinctUntilChanged());

    const netObervable = combineLatest(
      vcf,
      gross,
      (x, y) => ({ vcf: x, gross: y })
    ).pipe(map(x => {
      // check for nulls and empty strings
      if (x.vcf.value === null ||
        x.gross.value === null ||
        x.vcf.value === '' ||
        x.gross.value === '' ||
        (x.gross.initialData &&
        x.vcf.initialData)) {
          return null;
        }
      const parsedVcf = +x.vcf.value;
      const parsedGross = +x.gross.value;
      if (isNaN(parsedVcf) || isNaN(parsedGross)) {
        return null;
      }
      const newNet = parsedVcf * parsedGross;
      this._fieldBagService.addOrSetField('NetQuantity', newNet.toString(), 'MeterMathService');
      return newNet;
    }));

    return combineLatest(grossObservable, netObervable,
      (x, y) => ({ gross: x, net: y }));
  }

  reset(): void {
  }
}
