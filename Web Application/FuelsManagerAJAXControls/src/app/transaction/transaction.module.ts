import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { TransactionLayoutModificationComponent } from './transaction-layout-modification/transaction-layout-modification.component';
import { TransactionFieldPositionPlacementComponent } from './webparts/transaction-field-position-placement/transaction-field-position-placement.component';
import { AppModule } from '../app.module';
import { FMCoreModule } from '../fm-core/fm-core.module';
import { FormsModule } from '@angular/forms';
import { ClickOutsideModule } from 'ng-click-outside';
import { InsertTransactionComponent } from './insert-transaction/insert-transaction.component';
import { TransactionFieldWrapperComponent } from './webparts/transaction-field-wrapper/transaction-field-wrapper.component';
import { TransactionAliasInputComponent } from './webparts/fields/transaction-alias-input/transaction-alias-input.component';
import { TransactionAreaFormInputComponent } from './webparts/fields/transaction-area-form-input/transaction-area-form-input.component';
import { TransactionDateInputComponent } from './webparts/fields/transaction-date-input/transaction-date-input.component';
import { TransactionDateTimeInputComponent } from './webparts/fields/transaction-date-time-input/transaction-date-time-input.component';
import { TransactionFreeFormInputComponent } from './webparts/fields/transaction-free-form-input/transaction-free-form-input.component';
import { TransactionGenericListInputComponent } from './webparts/fields/transaction-generic-list-input/transaction-generic-list-input.component';
import { TransactionMeterGroupingInputComponent } from './webparts/fields/transaction-meter-grouping-input/transaction-meter-grouping-input.component';
import { TransactionVCFGroupingInputComponent } from './webparts/fields/transaction-vcfgrouping-input/transaction-vcfgrouping-input.component';
import { TransactionLabelComponent } from './webparts/fields/transaction-label/transaction-label.component';
import { TransactionLineComponent } from './webparts/fields/transaction-line/transaction-line.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { ViewSubmittedTransactionsComponent } from './view-submitted-transactions/view-submitted-transactions.component';
import { TransactionDocumentNumberInputComponent } from './webparts/fields/transaction-document-number-input/transaction-document-number-input.component';
import { TransactionCheckboxInputComponent } from './webparts/fields/transaction-checkbox-input/transaction-checkbox-input.component';

@NgModule({
  imports: [
    // NoopAnimationsModule,
    NgSelectModule,
    ClickOutsideModule,
    FormsModule,
    CommonModule,
    FMCoreModule,
    BrowserAnimationsModule
  ],
  exports: [TransactionLayoutModificationComponent],
  providers: [DecimalPipe],
  declarations: [TransactionLayoutModificationComponent, TransactionFieldPositionPlacementComponent,
    InsertTransactionComponent, TransactionFieldWrapperComponent, TransactionAliasInputComponent,
    TransactionAreaFormInputComponent, TransactionDateInputComponent, TransactionDateTimeInputComponent,
    TransactionFreeFormInputComponent, TransactionGenericListInputComponent,
    TransactionMeterGroupingInputComponent, TransactionVCFGroupingInputComponent, TransactionLabelComponent,
    TransactionLineComponent, ViewSubmittedTransactionsComponent, TransactionDocumentNumberInputComponent,
    TransactionCheckboxInputComponent],
  entryComponents: [TransactionFieldWrapperComponent, TransactionAliasInputComponent,
    TransactionAreaFormInputComponent, TransactionDateInputComponent, TransactionDateTimeInputComponent,
    TransactionFreeFormInputComponent, TransactionGenericListInputComponent,
    TransactionMeterGroupingInputComponent, TransactionVCFGroupingInputComponent, TransactionLabelComponent,
    TransactionLineComponent, ViewSubmittedTransactionsComponent, TransactionDocumentNumberInputComponent,
    TransactionCheckboxInputComponent]
})
export class TransactionModule { }
