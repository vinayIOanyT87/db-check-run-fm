import { Injectable } from '@angular/core';
import { BehaviorSubject, combineLatest, Observable, empty, interval } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { SiteService } from '../../fm-core/services/site.service';
import {
  filter,
  distinctUntilChanged,
  tap,
  mergeMap,
  delay,
  map,
  catchError,
  skipUntil,
  take
} from 'rxjs/operators';
import { VCFService } from '../../fm-core/services/vcf.service';
import { CurrentTransactionDetailsService } from './current-transaction-details.service';
import { CurrentTransactionFieldBagService } from './current-transaction-field-bag.service';

@Injectable({
  providedIn: 'root'
})
export class VCFFieldCalculatorService {
  private _vcf = new BehaviorSubject<VCFState>({
    value: null,
    isDisabled: false,
    error: null
  });
  private _temp = new BehaviorSubject<VCFState>({
    value: null,
    isDisabled: false,
    error: null
  });
  private _density = new BehaviorSubject<VCFState>({
    value: null,
    isDisabled: false,
    error: null
  });
  private _product = new BehaviorSubject<VCFState>({
    value: null,
    isDisabled: false,
    error: null
  });
  private _loadingVcfFromServer = new BehaviorSubject<boolean>(false);

  public vcf = this._vcf.asObservable();
  public temp = this._temp.asObservable();
  public density = this._density.asObservable();
  public product = this._product.asObservable();
  public loadingVcfFromServer = this._loadingVcfFromServer.asObservable();

  private _skipVCFNetworkCalls = true;

  constructor(private vcfService: VCFService, private transactionBag: CurrentTransactionFieldBagService) {}

  bootstrap(): Observable<void> {
    const vcfSubscription = this.vcf
      .pipe(distinctUntilChanged((x, y) => x.value === y.value))
      .pipe(map((x => {
        if (this._temp.value.value && this._density.value.value) {
          // if temp and value are set, do not muck around with the other fields
          return;
        }
        if (!(x === null || x === undefined) && !(x.value === null || x.value === undefined)) {
          // if vcf has a value, disable temp and density
          this._temp.next({ value: null, isDisabled: true, error: null });
          this._density.next({ value: null, isDisabled: true, error: null });
        } else {
          // enable temp and density
          this._temp.next({ value: null, isDisabled: false, error: null });
          this._density.next({ value: null, isDisabled: false, error: null });
        }
      })));

    const tempDensitySubscription = combineLatest(
      this.temp.pipe(distinctUntilChanged((x, y) => x.value === y.value)),
      this.density.pipe(distinctUntilChanged((x, y) => x.value === y.value)),
      this.product.pipe(distinctUntilChanged((x, y) => x.value === y.value)),
      (x, y, z) => ({ temp: x, density: y, product: z })
    )
      .pipe(filter(x => !this._skipVCFNetworkCalls))
      .pipe(
        tap(x => {
          if (
            (x.temp.value == null || x.density.value == null || x.product.value == null) &&
            this._vcf.value.isDisabled
          ) {
            // temp or density are empty, enable vcf
            this._vcf.next({ value: null, isDisabled: false, error: null });
          }
        })
      )
      .pipe(filter(x => x.temp.value != null && x.density.value != null && x.product.value != null))
      .pipe(
        tap(x => {
          this._loadingVcfFromServer.next(true);
        })
      )
      .pipe(
        tap(x => {
          this._vcf.next({ value: null, isDisabled: true, error: null });
        })
      )
      .pipe(
        mergeMap(x => {
          console.log(this.transactionBag);
          return this.vcfService.getVCF(x.product.value, +x.temp.value, +x.density.value);
        })
      )
      .pipe(delay(1000))
      .pipe(map((x) => {
        this._loadingVcfFromServer.next(false);
        this._vcf.next({ value: <any>x, isDisabled: true, error: null });
      }))
      .pipe(catchError((error, originalObservable) => {
        this._loadingVcfFromServer.next(false);
        this._vcf.next({
          value: 'Cannot calculate VCF',
          isDisabled: true,
          error: error
        });
        return empty();
      }));
      this.checkForDisabledFields();
      this.disableVcfUpdatesFor1Second();
      return combineLatest(this.vcf, tempDensitySubscription, vcfSubscription, (a, b, c) => null);
  }

  disableVcfUpdatesFor1Second(): void {
    this._skipVCFNetworkCalls = true;
    setTimeout(() => {
      this._skipVCFNetworkCalls = false;
    }, 1000);
  }

  checkForDisabledFields(): void {
    const density = this.transactionBag.getField('Density');
    const temp = this.transactionBag.getField('Temperature');
    const vcf = this.transactionBag.getField('Vcf');
    if (temp && density &&
      temp !== '' && density !== '') {
        this._vcf.next({value: vcf, isDisabled: true, error: null });
      }
  }

  setVcf(toSet: string): void {
    if (<any>toSet === '') {
      toSet = null;
    }
    this._vcf.next({ value: toSet, isDisabled: false, error: null });
  }

  setTemp(toSet: string): void {
    if (<any>toSet === '') {
      toSet = null;
    }
    this._temp.next({ value: toSet, isDisabled: false, error: null });
  }

  setDensity(toSet: string): void {
    if (<any>toSet === '') {
      toSet = null;
    }
    this._density.next({ value: toSet, isDisabled: false, error: null });
  }

  setProduct(toSet: string): void {
    if (<any>toSet === '') {
      toSet = null;
    }
    this._product.next({ value: toSet, isDisabled: false, error: null });
  }
}

export interface VCFState {
  value: string;
  isDisabled: boolean;
  error: any;
}
