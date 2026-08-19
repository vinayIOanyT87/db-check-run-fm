import { Injectable, ApplicationRef } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TransactionFieldChangedEvent } from './DTO/TransactionFieldChangedEvent';
import { filter } from 'rxjs/operators';
import { CurrentTransactionSettings } from './DTO/current-transaction-settings';

@Injectable({
  providedIn: 'root'
})
export class CurrentTransactionFieldBagService {
  private currentFields: any = {};
  public currentSettings: CurrentTransactionSettings = new CurrentTransactionSettings();
  public currentLists: any = {};
  private _hasChanged = new BehaviorSubject<TransactionFieldChangedEvent>(null);
  /**
   * This containts any changes to current transactions
   */
  public hasChanged = this._hasChanged.pipe(filter((x) => x != null));

  constructor() {
  }

  addOrSetField(field: string, value: string, source: string = null, initialData: boolean = false): void {
      this.currentFields[field] = value;
      if (source == null) {
        source = field;
      }
      this._hasChanged.next(new TransactionFieldChangedEvent({ field: field, value: value, source: source, initialData: initialData }));
  }

  addOrSetList(field: string, values: string[], source: string = null): void {
    this.currentLists[field] = values;
    if (source == null) {
      source = field;
    }
    this._hasChanged.next(new TransactionFieldChangedEvent({ field: field, list: values, source: source }));
  }

  getField(field: string): string {
      // https://stackoverflow.com/questions/1098040/checking-if-a-key-exists-in-a-javascript-object
      if (!(field in this.currentFields)) {
          this.addOrSetField(field, null, 'CurrentTransactionFieldBagService');
      }
      return this.currentFields[field];
  }

  getAllProperties(): any {
      return this.currentFields;
  }

  clear(): void {
      const currentTempFields = this.currentFields;
      this.currentFields = {};
      const that = this;
      Object.keys(currentTempFields).forEach(x => {
          that._hasChanged.next(new TransactionFieldChangedEvent({ field: x, value: null, source: 'CurrentTransactionFieldBagService'}));
      });
  }
}
