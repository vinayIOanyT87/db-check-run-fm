import { NgModule, ApplicationRef } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TransactionModule } from './transaction/transaction.module';
import { TransactionLayoutModificationComponent } from './transaction/transaction-layout-modification/transaction-layout-modification.component';
import { FormsModule } from '@angular/forms';
import { FMCoreModule } from './fm-core/fm-core.module';
import { InsertTransactionComponent } from './transaction/insert-transaction/insert-transaction.component';
import { FMLoadingScreenComponent } from './fm-core/webparts/fm-loading-screen/fm-loading-screen.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { APP_BASE_HREF, CommonModule } from '../../node_modules/@angular/common';
import { ViewSubmittedTransactionsComponent } from './transaction/view-submitted-transactions/view-submitted-transactions.component';
import { NgSelectModule } from '@ng-select/ng-select'

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    FormsModule,
    FMCoreModule,
    TransactionModule,
    NgbModule,
    NgSelectModule,
    CommonModule
  ],
  providers: [],
   entryComponents: [AppComponent, TransactionLayoutModificationComponent,
     InsertTransactionComponent, FMLoadingScreenComponent]
  // bootstrap: [SecondEntryPageComponent, AppComponent]
})
export class AppModule {
  ngDoBootstrap(appRef: ApplicationRef) {
    if (document.readyState === 'complete' || document.readyState === 'interactive') {
      this.bootstrap(appRef);
    } else {
      document.addEventListener('DOMContentLoaded', (event) => {
        this.bootstrap(appRef);
      });
    }
  }

  bootstrap(appRef: ApplicationRef) {
    let wasBootstrapeed = false;
    if (document.getElementsByTagName('app-root').length > 0) {
      wasBootstrapeed = true;
      appRef.bootstrap(AppComponent);
    }
    if (document.getElementsByTagName('app-transaction-layout-modification').length > 0) {
      wasBootstrapeed = true;
      appRef.bootstrap(TransactionLayoutModificationComponent);
    }
    if (document.getElementsByTagName('app-insert-transaction').length > 0) {
      wasBootstrapeed = true;
      appRef.bootstrap(InsertTransactionComponent);
    }
    if (document.getElementsByTagName('app-view-submitted-transactions').length > 0) {
      wasBootstrapeed = true;
      appRef.bootstrap(ViewSubmittedTransactionsComponent);
    }
    if (!wasBootstrapeed) {
    }
  }
 }
