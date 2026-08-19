import { Component, OnInit, Output, Input, OnDestroy, EventEmitter, ApplicationRef, OnChanges, SimpleChanges } from '@angular/core';
import { TransactionAliasPlacementInformation } from '../../../fm-core/DTO/transaction-alias-placement-information';

@Component({
  selector: 'app-transaction-field-position-placement',
  templateUrl: './transaction-field-position-placement.component.html',
  styleUrls: ['./transaction-field-position-placement.component.css']
})
export class TransactionFieldPositionPlacementComponent implements OnInit, OnDestroy, OnChanges {
  @Input() public transactionAliasPlacementInfoDTO: TransactionAliasPlacementInformation;
  @Output() public TransactionFieldChanged = new EventEmitter();
  public displayOptions = false;
  public showOptionPersistDataOnSave = false;

  public get row(): number { return this.transactionAliasPlacementInfoDTO.rowSpan + 1; }
  public set row(fieldValue: number) {
      if (fieldValue) {
        this.transactionAliasPlacementInfoDTO.rowSpan = fieldValue - 1;
      }
  }

  public get column(): number { return this.transactionAliasPlacementInfoDTO.columnSpan + 1; }
  public set column(fieldValue: number) {
      if (fieldValue) {
        this.transactionAliasPlacementInfoDTO.columnSpan = fieldValue - 1;
      }
  }

  constructor(private applicationRef: ApplicationRef) { }

  ngOnChanges(changes: SimpleChanges): void {
    this.setPersistOnSaveOption();
  }

  isStringEmptyOrNull(value: string): boolean {
      return (!value || value === undefined || value === '' || value.length === 0);
  }

  setPersistOnSaveOption(): void {
    if (this.transactionAliasPlacementInfoDTO
        && !this.transactionAliasPlacementInfoDTO.isLabel
        && !this.transactionAliasPlacementInfoDTO.isLine) {
          this.showOptionPersistDataOnSave = true;
    }
  }

  showOptions(): void {
      this.displayOptions = true;
      this.applicationRef.tick();
  }

  hideOptions(): void {
      this.displayOptions = false;
      this.TransactionFieldChanged.emit();
  }

  validate(): void {
  }

  ngOnDestroy(): void { }
  ngOnInit(): void { }
}

