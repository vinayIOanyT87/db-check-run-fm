import { Component, OnInit, OnDestroy } from '@angular/core';
import { LocalStorageService } from '../../fm-core/services/local-storage.service';
import { TransactionStoredEntry } from '../../fm-core/DTO/transaction-alias-stored-fields-entry';

@Component({
  selector: 'app-view-submitted-transactions',
  templateUrl: './view-submitted-transactions.component.html',
  styleUrls: ['./view-submitted-transactions.component.css']
})
export class ViewSubmittedTransactionsComponent implements OnInit, OnDestroy {
  public storedEntries: TransactionStoredEntry[] = [];
  public selectedEntry: TransactionStoredEntry = null;
  constructor(private localStorage: LocalStorageService) { }

  getDocumentNumber(toFind: TransactionStoredEntry): string {
    return toFind.sentEntry.DocumentNumber;
  }
  timeDifferenceInMilli(toDiff: TransactionStoredEntry): number {
    const started = new Date(toDiff.timeStarted);
    const ended = new Date(toDiff.timeComplete);
    return Math.abs(started.getTime() - ended.getTime());
  }

  clearAll(): void {
    this.localStorage.store('CurrentStoredTransactions', null);
    this.ngOnInit();
  }

  selectTransaction(transaction: TransactionStoredEntry) {
    this.selectedEntry = transaction;
  }

  ngOnDestroy(): void {}
  ngOnInit(): void {
    this.storedEntries = this.localStorage.get('CurrentStoredTransactions');
  }
}
