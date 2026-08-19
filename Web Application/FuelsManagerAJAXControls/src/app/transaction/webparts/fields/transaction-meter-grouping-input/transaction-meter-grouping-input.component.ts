import { Component, OnInit, OnDestroy, Input, ChangeDetectorRef } from '@angular/core';
import { ITransactionAliasField } from '../itransaction-alias-field';
import { TransactionAliasField } from '../../../../fm-core/DTO/transaction-alias-placement-information';
import { Subscription } from 'rxjs';
import { MeterMathService } from '../../../services/meter-math.service';
import { CurrentTransactionFieldBagService } from '../../../services/current-transaction-field-bag.service';
import { filter } from 'rxjs/operators';
import { formatNumber, DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-transaction-meter-grouping-input',
  templateUrl: './transaction-meter-grouping-input.component.html',
  styleUrls: ['./transaction-meter-grouping-input.component.css']
})
export class TransactionMeterGroupingInputComponent implements ITransactionAliasField, OnInit, OnDestroy {
  @Input() public TransactionAliasField: TransactionAliasField;
  @Input() public currentFocusLabel: string;
  @Input() public canEdit: boolean;
  public required = false;

  public label = '';
  public fieldName = '';

  private _fieldContents = '';
  public set fieldContents(value: string) {
    if (value === '') { this._fieldContents = ''; } else {
      if (this._fieldContents != null && this._fieldContents['replace']) {
        this._fieldContents = this._fieldContents.replace(/,/g, '');
      }
      const parsedFieldContents = +this._fieldContents;
      if (!isNaN(parsedFieldContents)) {
          if (value != null && value['replace']) {
            value = value.replace(/,/g, '');
          }
          this._fieldContents = value;
          this._fieldBagService.addOrSetField(this.fieldName, <any>this._fieldContents);
      }
    }
  }
  public get fieldContents(): string {
    const parsedFieldContents = +this._fieldContents;
    if (isNaN(parsedFieldContents) || this._fieldContents === '') {
      return '';
    }
    return this._decimalPipe.transform(parsedFieldContents, `1.${this.decimalPrecision}-${this.decimalPrecision}` ).replace(/,/g, '');
  }
  public isDisabled = false;

  private decimalPrecision: number = null;
  private fieldUpdateSubscription = new Subscription;
  private decimalUpdateSubscription = new Subscription;
  constructor(private _meterMathService: MeterMathService,
      private _fieldBagService: CurrentTransactionFieldBagService,
      private _changeRef: ChangeDetectorRef,
      private _decimalPipe: DecimalPipe) { }

  isValid(): boolean {
    return true;
  }

  ngOnInit(): void {
      if ((this.TransactionAliasField === null || this.TransactionAliasField === undefined)) {
          return;
      }
      this.label = this.TransactionAliasField.DisplayName;
      this.fieldName = this.TransactionAliasField.ID;
      this.required = this.TransactionAliasField.FieldRequired;

      this.fieldUpdateSubscription = this._fieldBagService.hasChanged
        .pipe(filter((x) => x.field === this.fieldName))
        .pipe(filter((x) => x.source !== this.fieldName))
        .subscribe((x) => {
          if (x.value === null || x.value === '') {
            this._fieldContents = '';
          } else if (!isNaN(+x.value)) {
            this._fieldContents = x.value;
          }
          this._changeRef.detectChanges();
        });

      this.decimalPrecision = this._fieldBagService.currentSettings.volumeDecimalPrecision;
      this.decimalUpdateSubscription = this._fieldBagService.hasChanged
      .pipe(filter((x) => x.field === 'Product'))
      .subscribe((x) => {
        if (x.value === null || x.value === '') {
          this.decimalPrecision = this._fieldBagService.currentSettings.volumeDecimalPrecision;
        } else {
          const product = this._fieldBagService.currentSettings.products.find(element => element.ID === x.value);
          if (!product) {
            throw new Error();
          }
          this.decimalPrecision = product.VolumeDecimalPlaces;
          this.fieldContents = this._fieldContents;
        }
        this._changeRef.detectChanges();
      });
  }
  ngOnDestroy(): void {
      this.fieldUpdateSubscription.unsubscribe();
      this.decimalUpdateSubscription.unsubscribe();
  }
}
