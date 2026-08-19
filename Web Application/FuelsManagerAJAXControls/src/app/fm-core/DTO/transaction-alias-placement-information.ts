export interface TransactionDetailsDTO {
  TransactionFields: TransactionAliasField[];
  AutoDocumentNumber: boolean;
  FieldsWithLists: FieldWithAssociatedList[];
  AllProducts: ProductDTO[];
  VolumeDecimalPrecision: number;
  TemperatureDecimalPlaces: number;
  DensityDecimalPlaces: number;
  TransactionAliasType: number;
}

export interface ProductDTO {
  ID: string;
  VolumeDecimalPlaces: number;
  TemperatureDecimalPlaces: number;
  DensityDecimalPlaces: number;
}

export interface ColumnDefinition {
  TableName: string;
  ColumnName: string;
  ColumnType: string;
  PropertyName: string;
  MaxLength?: number;
  HasListAttached: boolean;
}

export interface TransactionAliasField {
  Type: string;
  ID: string;
  ClearOnNew: boolean;
  DispatchField: boolean;
  VirtualField: boolean;
  FieldRequired: boolean;
  DisplayName: string;
  UserGroupId?: any;
  DisplayOrder: number;
  IdentityGuid: string;
  AliasName: string;
  DbName: string;
  ColumnDefinition: ColumnDefinition;
}

export interface TransactionAliasPlacementInformation {
  id: string;
  identityGuid: string;
  displayName: string;
  originalDisplayOrder: number;
  xPosition: number;
  yPosition: number;
  isPlaced: boolean;
  rowSpan: number;
  columnSpan: number;
  isLabel: boolean;
  labelContents: string;
  isLine: boolean;
  persistDataAfterSave: boolean;
}

export interface FieldWithAssociatedList {
  FieldName: string;
  Options: string[];
}
