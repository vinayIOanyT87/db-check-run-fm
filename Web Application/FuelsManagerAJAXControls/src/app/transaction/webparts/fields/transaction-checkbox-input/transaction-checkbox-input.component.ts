import { Component, OnInit, Input, ChangeDetectorRef } from '@angular/core';
import { ITransactionAliasField } from '../itransaction-alias-field';
import { TransactionAliasField } from 'src/app/fm-core/DTO/transaction-alias-placement-information';
import { CurrentTransactionFieldBagService } from 'src/app/transaction/services/current-transaction-field-bag.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-transaction-checkbox-input',
  templateUrl: './transaction-checkbox-input.component.html',
  styleUrls: ['./transaction-checkbox-input.component.css']
})
export class TransactionCheckboxInputComponent
  implements ITransactionAliasField, OnInit {
  @Input() TransactionAliasField: TransactionAliasField;
  @Input() currentFocusLabel: string;
  @Input() canEdit: boolean;

  public label = '';
  public fieldName = '';
  public required = false;

  private _fieldContents = false;
  public get fieldContents() {
    return this._fieldContents;
  }
  public set fieldContents(value: boolean) {
    this._fieldContents = value;
    this._fieldBagService.addOrSetField(
      this.fieldName,
      String(this._fieldContents)
    );
  }

  constructor(private _fieldBagService: CurrentTransactionFieldBagService,
    private _changeRef: ChangeDetectorRef) {}

  isValid(): boolean {
    return true;
  }
  ngOnInit() {
    if (this.TransactionAliasField) {
      this.label = this.TransactionAliasField.DisplayName;
      this.fieldName = this.TransactionAliasField.ID;
      this.required = this.TransactionAliasField.FieldRequired;
      this.fieldContents = (this._fieldBagService.getField(this.fieldName) === 'true');

      if (this.fieldContents === null) {
        this.fieldContents = false;
      }
      this._fieldBagService.hasChanged
        .pipe(filter(x => x.field === this.fieldName))
        .pipe(filter(x => x.source !== this.fieldName))
        .subscribe(x => {
          if (x.value == null) {
            x.value = '';
          }
          this._fieldContents = (x.value === 'true');
          this._changeRef.detectChanges();
        });
    }
  }
}
