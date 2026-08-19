import { Component, AfterViewInit, Input, OnDestroy, ViewChild, ComponentFactoryResolver, ViewContainerRef, ComponentRef, OnChanges, SimpleChanges } from '@angular/core';
import { TransactionAliasField, TransactionAliasPlacementInformation } from '../../../fm-core/DTO/transaction-alias-placement-information';
import { TransactionAliasInputComponent } from '../fields/transaction-alias-input/transaction-alias-input.component';
import { TransactionDateInputComponent } from '../fields/transaction-date-input/transaction-date-input.component';
import { TransactionVCFGroupingInputComponent } from '../fields/transaction-vcfgrouping-input/transaction-vcfgrouping-input.component';
import { TransactionMeterGroupingInputComponent } from '../fields/transaction-meter-grouping-input/transaction-meter-grouping-input.component';
import { TransactionGenericListInputComponent } from '../fields/transaction-generic-list-input/transaction-generic-list-input.component';
import { TransactionDateTimeInputComponent } from '../fields/transaction-date-time-input/transaction-date-time-input.component';
import { TransactionAreaFormInputComponent } from '../fields/transaction-area-form-input/transaction-area-form-input.component';
import { TransactionFreeFormInputComponent } from '../fields/transaction-free-form-input/transaction-free-form-input.component';
import { ITransactionAliasField } from '../fields/itransaction-alias-field';
import { FieldHostDirective } from '../../../fm-core/directives/field-host.directive';
import { TransactionLabelComponent } from '../fields/transaction-label/transaction-label.component';
import { TransactionLineComponent } from '../fields/transaction-line/transaction-line.component';
import { TransactionDocumentNumberInputComponent } from '../fields/transaction-document-number-input/transaction-document-number-input.component';
import { TransactionCheckboxInputComponent } from '../fields/transaction-checkbox-input/transaction-checkbox-input.component';

@Component({
  selector: 'app-transaction-field-wrapper',
  templateUrl: './transaction-field-wrapper.component.html',
  styleUrls: ['./transaction-field-wrapper.component.css']
})
export class TransactionFieldWrapperComponent implements AfterViewInit, OnDestroy, OnChanges {
  @Input() public TransactionAliasField: TransactionAliasField;
  @Input() public transactionAliasPlacementInfoDTO: TransactionAliasPlacementInformation;
  @Input() public canEdit: boolean;
  @Input() public currentFocusLabel: string;

  public showLabel = false;
  public showLine = false;
  public showDynamicComponent = true;
  @ViewChild(FieldHostDirective, {static: false}) public fieldHost: FieldHostDirective;

  private currentInstance: ITransactionAliasField;

  constructor(private _componentFactoryResolver: ComponentFactoryResolver,
      private viewContainerRef: ViewContainerRef) { }

  isStringEmptyOrNull(value: string): boolean {
      return (!value || value === undefined || value === '' || value.length === 0);
  }

  ngAfterViewInit(): void {
      if ((this.fieldHost === null || this.fieldHost === undefined)) {
          return;
      }
      if (!(this.transactionAliasPlacementInfoDTO === null || this.transactionAliasPlacementInfoDTO === undefined) &&
          this.transactionAliasPlacementInfoDTO.isLabel) {
          // its a label
          const createdLabelComponent = this.createComponentOnTemplate(TransactionLabelComponent);
          (<TransactionLabelComponent>createdLabelComponent.instance).label =
              this.transactionAliasPlacementInfoDTO.labelContents;
      } else if (!(this.transactionAliasPlacementInfoDTO === null || this.transactionAliasPlacementInfoDTO === undefined) &&
          this.transactionAliasPlacementInfoDTO.isLine) {
          // its a label
          const createdLabelComponent = this.createComponentOnTemplate(TransactionLineComponent);
      } else if (!(this.TransactionAliasField === null || this.TransactionAliasField === undefined)) {
          const typeToCreate = this.createTransactionAliasFieldType(this.TransactionAliasField);
          const createdLabelComponent = this.createComponentOnTemplate(typeToCreate);
          this.currentInstance = createdLabelComponent.instance;
          this.currentInstance.TransactionAliasField = this.TransactionAliasField;
          this.currentInstance.canEdit = this.canEdit;
          this.currentInstance.currentFocusLabel = this.currentFocusLabel;
      }
  }

  createTransactionAliasFieldType(field: TransactionAliasField): any {
    // special field behavior
    switch (field.ID) {
      case 'AliasName':
        return TransactionAliasInputComponent;
      case 'InventoryDate':
        return TransactionDateInputComponent;
      // case 'Product':
      //    return TransactionProductInputComponent;
      case 'Vcf':
      case 'Temperature':
      case 'Density':
        return TransactionVCFGroupingInputComponent;
      case 'MeterStart':
      case 'MeterStop':
      case 'GrossQuantity':
      case 'NetQuantity':
        return TransactionMeterGroupingInputComponent;
      case 'MeterStartDateTime':
      case 'MeterStopDateTime':
        return TransactionDateTimeInputComponent;
      case 'Notes':
        return TransactionAreaFormInputComponent;
      case 'DocumentNumber':
        return TransactionDocumentNumberInputComponent;
    }

    if (field.ColumnDefinition.HasListAttached) {
      return TransactionGenericListInputComponent;
    }

    // capture any types that only have one type of field - ie bits = checkboxes
    switch (field.ColumnDefinition.ColumnType) {
      case 'bit':
      return TransactionCheckboxInputComponent;
    }
    // nothing matches, it is a text box
    return TransactionFreeFormInputComponent;
  }

  createComponentOnTemplate(componentToCreate: any): ComponentRef<any> {
      return this.fieldHost.viewContainerRef.createComponent(componentToCreate);
  }

  /**
   * propogates changes to child reference
   */
  ngOnChanges(changes: SimpleChanges): void {
    if ((this.currentInstance === null || this.currentInstance === undefined)) {
      return;
    }
    if (!(changes['TransactionAliasField'] === null || changes['TransactionAliasField'] === undefined) &&
          changes['TransactionAliasField'].currentValue !== changes['TransactionAliasField'].previousValue) {
      this.currentInstance.TransactionAliasField = changes['TransactionAliasField'].currentValue;
    }
    if (!(changes['currentFocusLabel'] === null || changes['currentFocusLabel'] === undefined) &&
        changes['currentFocusLabel'].currentValue !== changes['currentFocusLabel'].previousValue) {
      this.currentInstance.currentFocusLabel = changes['currentFocusLabel'].currentValue;
    }
    if (!(changes['canEdit'] === null || changes['canEdit'] === undefined) &&
        changes['canEdit'].currentValue !== changes['canEdit'].previousValue) {
      this.currentInstance.canEdit = changes['canEdit'].currentValue;
    }
  }

  ngOnDestroy(): void {
  }
}
