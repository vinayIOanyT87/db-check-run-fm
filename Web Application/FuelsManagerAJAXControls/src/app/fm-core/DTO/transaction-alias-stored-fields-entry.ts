import { TransactionAliasPlacementInformation } from './transaction-alias-placement-information';

export interface TransactionStoredEntry {
  sentEntry: any;
  sentTransactionAliasGuid: string;
  returnedEntry: any;
  isSent: any;
  timeStarted: Date;
  timeComplete: Date;
  error: any;
}

export interface TransactionAliasStoredFieldsEntry {
  aliasGuid: string;
  numberOfColumns: number;
  numberOfRows: number;
  retrieveMeterStartOnMeterIDSet: boolean;
  fieldMap: TransactionAliasPlacementInformation[];
}
