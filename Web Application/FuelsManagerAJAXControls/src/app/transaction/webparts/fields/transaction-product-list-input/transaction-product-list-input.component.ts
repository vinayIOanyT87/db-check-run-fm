import { Component, OnInit, OnDestroy, Input, ApplicationRef, ViewChild, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { ITransactionAliasField } from '../itransaction-alias-field';
import { TransactionAliasField } from '../../../../fm-core/DTO/transaction-alias-placement-information';
import { Subscription } from 'rxjs';
import { TransactionService } from '../../../../fm-core/services/transaction.service';
import { CurrentTransactionFieldBagService } from '../../../services/current-transaction-field-bag.service';
import { VCFFieldCalculatorService } from '../../../services/vcffield-calculator.service';
import { NgSelectComponent } from '@ng-select/ng-select';
import { filter, tap, startWith } from 'rxjs/operators';
import { TransactionFieldChangedEvent } from '../../../services/DTO/TransactionFieldChangedEvent';

@Component({
  selector: 'app-transaction-product-list-input',
  templateUrl: './transaction-product-list-input.component.html',
  styleUrls: ['./transaction-product-list-input.component.css']
})
export class TransactionProductListInputComponent implements ITransactionAliasField,
  OnInit, OnDestroy {
  private _currentFocusLabel: string;
  @Input() public TransactionAliasField: TransactionAliasField;
  @Input() public set currentFocusLabel(value: string) {
    this._currentFocusLabel = value;
    if (value !== '') {
      this.focusCheck();
    }
  }
  public get currentFocusLabel(): string {
    return this._currentFocusLabel;
  }
  @Input() public canEdit: boolean;

  @ViewChild('dropDown', {static: false}) public dropDown: NgSelectComponent;

  public label = '';
  public fieldName = '';
  public listOfItems: string[] = [];
  public required = false;
  private _pickedItem: string = null;
  private partialValue: string = null;

  public get pickedItem(): string { return this._pickedItem; }
  public set pickedItem(picked: string) {
    this._pickedItem = picked;
    this._fieldBagService.addOrSetField(this.fieldName, this._pickedItem);
  }

  private initialListSubscription = new Subscription();
  constructor(private _transactionService: TransactionService,
    private _fieldBagService: CurrentTransactionFieldBagService,
    private _appRef: ApplicationRef,
    private _vcfService: VCFFieldCalculatorService) {
  }

  isValid(): boolean {
    return true;
  }

  ngOnInit(): void {
    if (this.TransactionAliasField) {
      this.label = this.TransactionAliasField.DisplayName;
      this.fieldName = this.TransactionAliasField.ID;

      this.required = this.TransactionAliasField.FieldRequired;

      this.focusCheck();

      this.pickedItem = this._fieldBagService.getField(this.fieldName);
      this.listOfItems = this._fieldBagService.currentLists[this.fieldName];
      this.pickDefaultIfRequirementsMet();
      this._fieldBagService.hasChanged
        .pipe(filter((x) => x.field === this.fieldName))
        .pipe(filter((x) => x.source !== this.fieldName))
        .pipe(filter((x) => x.value !== this.pickedItem))
        .subscribe((x) => {
          if (x.list !== null) {
            this.listOfItems = x.list;
            this.pickDefaultIfRequirementsMet();
            setTimeout(() => { this._appRef.tick(); }, 0);
            return;
          }
          if (x.value === null) {
            x.value = '';
          }
          this.pickedItem = x.value;
          this.pickDefaultIfRequirementsMet();
          setTimeout(() => { this._appRef.tick(); }, 0);
        });
    }
  }

  pickDefaultIfRequirementsMet(): void {
    if (this.listOfItems.length === 1 &&
      this.TransactionAliasField.FieldRequired) {
      this.pickedItem = this.listOfItems[0];
    }
  }

  ngOnDestroy(): void {
    this.initialListSubscription.unsubscribe();
  }
  clear(): void {
    this.partialValue = '';
  }
  savePartial(partialValue: string, key: string): void {
    if (key === 'Tab' ||
      key === 'Shift') {
      return;
    }
    this.partialValue = partialValue;
  }

  selectPartial() {
    if ((this.partialValue === null || this.partialValue === undefined) || this.partialValue === '') {
      return;
    }
    const firstMatchedItem = this.listOfItems.find(x => {
      return x.toLowerCase().includes(this.partialValue.toLowerCase());
    });
    this.pickedItem = firstMatchedItem;
    this.partialValue = null;
  }

  private focusCheck(): void {
    if (this.fieldName === this.currentFocusLabel) {
      this.dropDown.focus();
    }
  }

  public update(): void {
    this._appRef.tick();
  }

  public updateVcfGroup(): void {
    this._vcfService.setProduct(this.pickedItem);
  }
}
