import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { TransactionAliasField } from '../../../../fm-core/DTO/transaction-alias-placement-information';
import { ITransactionAliasField } from '../itransaction-alias-field';
import { CurrentTransactionFieldBagService } from '../../../services/current-transaction-field-bag.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-transaction-document-number-input',
  templateUrl: './transaction-document-number-input.component.html',
  styleUrls: ['./transaction-document-number-input.component.css']
})
export class TransactionDocumentNumberInputComponent implements ITransactionAliasField, OnInit {
  @Input() public TransactionAliasField: TransactionAliasField;
  @Input() public currentFocusLabel: string;
  @Input() public canEdit: boolean;

  public label = '';
  public fieldName = '';
  public automaticDocumentNumber = false;
  public required = false;
  private _fieldContents = '';
  public set fieldContents(value: string) {
    this._fieldContents = value;
    this._fieldBagService.addOrSetField(this.fieldName, this._fieldContents);
  }
  public get fieldContents(): string {
    if (this.automaticDocumentNumber) {
      return '{Auto Generated}';
    }
    return this._fieldContents;
  }
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
      this.fieldContents = this._fieldBagService.getField(this.fieldName);
      this.required = this.TransactionAliasField.FieldRequired;
      this._fieldBagService.hasChanged
        .pipe(filter((x) => x.field === this.fieldName))
        .pipe(filter((x) => x.source !== this.fieldName))
        .subscribe((x) => {
          if (x.value == null) {
            x.value = '';
          }

          if (this._fieldBagService.currentSettings.automaticDocumentNumber  &&
            !this._fieldBagService.currentSettings.existingTransaction) {
              this.automaticDocumentNumber = true;
          }

          this._fieldContents = x.value;
          this._changeRef.detectChanges();
        });
    }
  }

}
