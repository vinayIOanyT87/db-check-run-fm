import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { ITransactionAliasField } from '../itransaction-alias-field';
import { TransactionAliasField } from '../../../../fm-core/DTO/transaction-alias-placement-information';
import { CurrentTransactionFieldBagService } from '../../../services/current-transaction-field-bag.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-transaction-area-form-input',
  templateUrl: './transaction-area-form-input.component.html',
  styleUrls: ['./transaction-area-form-input.component.css']
})
export class TransactionAreaFormInputComponent implements ITransactionAliasField, OnInit {
  @Input() public TransactionAliasField: TransactionAliasField;
  @Input() public currentFocusLabel: string;
  @Input() public canEdit: boolean;
  public required = false;

  public label = '';
  public fieldName = '';
  private _fieldContents = '';
  public set fieldContents(value: string) {
      this._fieldContents = value;
      this._fieldBagService.addOrSetField(this.fieldName, this._fieldContents);
  }
  public get fieldContents(): string { return this._fieldContents; }
  constructor(private _fieldBagService: CurrentTransactionFieldBagService,
    private _changeRef: ChangeDetectorRef) { }

  isValid(): boolean {
    return true;
  }

  ngOnInit(): void {
      if (this.TransactionAliasField) {
          // this.label = this.TransactionAliasField.DisplayName;
          this.label = this.TransactionAliasField.DisplayName;
          this.fieldName = this.TransactionAliasField.ID;
          this.required = this.TransactionAliasField.FieldRequired;
          this.fieldContents = this._fieldBagService.getField(this.fieldName);
          this._fieldBagService.hasChanged
          .pipe(filter((x) => x.field === this.fieldName))
          .pipe(filter((x) => x.source !== this.fieldName))
          .subscribe((x) => {
            if (x.value == null) {
              x.value = '';
            }
            this.fieldContents = x.value;
            this._changeRef.detectChanges();
          });
      }
  }
}
