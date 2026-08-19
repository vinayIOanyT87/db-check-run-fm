import { Component, OnInit, OnDestroy, Input, ChangeDetectorRef } from '@angular/core';
import { ITransactionAliasField } from '../itransaction-alias-field';
import { TransactionAliasField } from '../../../../fm-core/DTO/transaction-alias-placement-information';
import { CurrentTransactionFieldBagService } from '../../../services/current-transaction-field-bag.service';
import { filter } from 'rxjs/operators';
import * as moment from 'moment';

@Component({
  selector: 'app-transaction-date-input',
  templateUrl: './transaction-date-input.component.html',
  styleUrls: ['./transaction-date-input.component.css']
})
export class TransactionDateInputComponent implements ITransactionAliasField, OnInit, OnDestroy {
  @Input() public TransactionAliasField: TransactionAliasField;
  @Input() public currentFocusLabel: string;
  @Input() public canEdit: boolean;

  public label = '';
  public fieldName = '';
  private _fieldValue: moment.Moment;
  public get fieldValue(): string {  return this._fieldValue.format('MM/DD/YYYY'); }
  public set fieldValue(fieldValue: string) {
      if (!(fieldValue === null || fieldValue === undefined)) {
          this._fieldValue = moment.utc(fieldValue);
          this._fieldBagService.addOrSetField(this.fieldName, this._fieldValue.toISOString(true));
      }
  }
  private skipInitilizingField = true;

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
          this._fieldBagService.hasChanged
          .pipe(filter((x) => x.field === this.fieldName))
          .pipe(filter((x) => x.source !== this.fieldName))
          .subscribe((x) => {
            if (x.value === null || x.value === '') {
              x.value = new Date().toISOString();
            }
            this._fieldValue = moment.utc(x.value);
            this._changeRef.detectChanges();
          });
        setTimeout(() => {
            const a = (<any>window).$('#' + this.fieldName);
            (<any>window).$('#' + this.fieldName).datepicker();
            (<any>window).$('#' + this.fieldName).change(() => {
              const inputValue = (<any>window).$('#' + this.fieldName).val();
              console.log(inputValue);
              this.fieldValue = moment.utc(inputValue, 'MM/DD/YYYY').toISOString(false);
              console.log(this.fieldValue);
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
