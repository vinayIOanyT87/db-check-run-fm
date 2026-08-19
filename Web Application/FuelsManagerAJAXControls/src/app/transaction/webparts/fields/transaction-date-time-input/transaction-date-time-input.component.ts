import { Component, OnInit, OnDestroy, Input, ChangeDetectorRef } from '@angular/core';
import { ITransactionAliasField } from '../itransaction-alias-field';
import { TransactionAliasField } from '../../../../fm-core/DTO/transaction-alias-placement-information';
import { CurrentTransactionFieldBagService } from '../../../services/current-transaction-field-bag.service';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import * as moment from 'moment';

@Component({
  selector: 'app-transaction-date-time-input',
  templateUrl: './transaction-date-time-input.component.html',
  styleUrls: ['./transaction-date-time-input.component.css']
})
export class TransactionDateTimeInputComponent implements ITransactionAliasField, OnInit, OnDestroy {
  @Input() public TransactionAliasField: TransactionAliasField;
  @Input() public currentFocusLabel: string;
  @Input() public canEdit: boolean;

  public label = '';
  public fieldName = '';
  private _fieldValue: moment.Moment;
  private transactionBagSubscription = Subscription.EMPTY;
  public get fieldValue(): string {  return this._fieldValue.format('MM/DD/YYYY'); }
  public set fieldValue(fieldValue: string) {
      this._fieldValue = moment.utc(fieldValue);
      if (!(fieldValue === null || fieldValue === undefined)) {
          this._fieldBagService.addOrSetField(this.fieldName, this._fieldValue.toISOString(true));
      }
  }
  public get fieldTimeValue(): string {
    if (this._fieldValue == null) {
      return '';
    }
    return moment.utc(this._fieldValue).format('h:mm a');
  }
  public set fieldTimeValue(fieldValue: string) {
      if (!(this._fieldValue === null || this._fieldValue === undefined)) {
        const time = moment.utc(fieldValue, 'h:mm a');
        const current = this._fieldValue;
        current.hour(time.hour());
        current.minute(time.minute());
        this._fieldValue = current;
        this._fieldBagService.addOrSetField(this.fieldName, current.toISOString(true));
        this._changeRef.markForCheck();
        this._changeRef.detectChanges();
      }
  }

  constructor(private _fieldBagService: CurrentTransactionFieldBagService,
    private _changeRef: ChangeDetectorRef) {
  }

  isValid(): boolean {
    return true;
  }

  ngOnInit(): void {
      if (this.TransactionAliasField) {
          this.label = this.TransactionAliasField.DisplayName;
          this.fieldName = this.TransactionAliasField.ID;
          this.setupInitialField();
          this.transactionBagSubscription = this._fieldBagService.hasChanged
          .pipe(filter((x) => x.field === this.fieldName))
          .pipe(filter((x) => x.source !== this.fieldName))
          .subscribe((x) => {
            if (x.value == null || x.value === '') {
              x.value = new Date().toISOString();
            }
            this._fieldValue = moment.utc(x.value);
            this._changeRef.detectChanges();
          });
          setTimeout(() => {
            (<any>window).$('#' + this.fieldName).datepicker();
            (<any>window).$('#' + this.fieldName).change(() => {
              const inputValue = (<any>window).$('#' + this.fieldName).val();
              this.fieldValue = moment.utc(inputValue, 'MM/DD/YYYY').toISOString(false);
              this._changeRef.detectChanges();
            });
          }, 0);
      }
  }

  setupInitialField(): void {
      const previousField = this._fieldBagService.getField(this.fieldName);
      if ((previousField === null || previousField === undefined)) {
          this._fieldValue = moment.utc();
      } else {
          this._fieldValue = moment.utc(previousField);
      }
  }

  ngOnDestroy(): void {
  }
}
