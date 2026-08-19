import { TransactionAliasField } from '../../../fm-core/DTO/transaction-alias-placement-information';

export interface ITransactionAliasField {
  TransactionAliasField: TransactionAliasField;
  currentFocusLabel: string;
  canEdit: boolean;
  isValid(): boolean;
}
