import { ProductDTO } from '../../../fm-core/DTO/transaction-alias-placement-information';

export class CurrentTransactionSettings {
  public automaticDocumentNumber = false;
  public existingTransaction = false;
  public products: ProductDTO[] = null;
  public volumeDecimalPrecision: number = null;
  public temperatureDecimalPrecision: number = null;
  public densityDecimalPrecision: number = null;
  public transactionAliasGuid = '';
  public transactionAliasType: number = null;
}
