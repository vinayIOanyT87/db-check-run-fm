import { Component, OnInit, Input } from '@angular/core';
import { ITransactionAliasField } from '../itransaction-alias-field';
import { TransactionAliasField } from '../../../../fm-core/DTO/transaction-alias-placement-information';
import { CurrentTransactionFieldBagService } from '../../../services/current-transaction-field-bag.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-transaction-alias-input',
  templateUrl: './transaction-alias-input.component.html',
  styleUrls: ['./transaction-alias-input.component.css']
})
export class TransactionAliasInputComponent implements ITransactionAliasField, OnInit {
  @Input() public TransactionAliasField: TransactionAliasField;
  @Input() public currentFocusLabel: string;
  @Input() public canEdit: boolean;

  public label = '';
  public fieldName = '';
  public aliasName = '';
  constructor(private _fieldBagService: CurrentTransactionFieldBagService) { }

  isValid(): boolean {
    return true;
  }

  ngOnInit(): void {
      if (this.TransactionAliasField) {
          this.label = this.TransactionAliasField.DisplayName;
          this.fieldName = this.TransactionAliasField.ID;
          this.aliasName = this.TransactionAliasField.AliasName;
          this._fieldBagService.addOrSetField(this.TransactionAliasField.ID, this.aliasName);
          this._fieldBagService.hasChanged
          .pipe(filter(x => x.field === this.fieldName))
          .pipe(filter(x => x.source !== this.fieldName))
          .subscribe(x => {
            this._fieldBagService.addOrSetField(this.TransactionAliasField.ID, this.aliasName);
          });
      }
  }
}

