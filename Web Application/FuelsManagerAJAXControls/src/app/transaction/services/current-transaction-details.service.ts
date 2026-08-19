import { Injectable } from '@angular/core';

declare var transactionAliasName: string;
declare var existingTransactionGuid: string;
/** modifyTransaction will be false for new transactions, true for existing transactions  */
declare var modifyTransaction: string;
declare var previousUrl: string;
declare var extendedAddScenario: boolean;
declare var prepopulateFollowingObject: any;

@Injectable({
  providedIn: 'root'
})
export class CurrentTransactionDetailsService {
  constructor() {}
  getTransactionAliasName(): string {
    return transactionAliasName;
  }
  getExistingTransactionGuid(): string {
    return existingTransactionGuid;
  }
  getModifyTransaction(): boolean {
    if (modifyTransaction &&
        modifyTransaction === 'true') {
          return true;
        }
    return false;
  }
  getPreviousUrl(): string {
    return previousUrl;
  }

  isExtendedAddScenario(): boolean {
    if (typeof(extendedAddScenario) == "undefined") {
      return false;
    }

    return extendedAddScenario;
  }

  presetFollowingFields(): any {
    return prepopulateFollowingObject;
  }

}
