

export interface ModifyTransactionSecurityRights {
  Defuel: number;
  Issue: number;
}

export interface ViewTransactionSecurityRights {
  Defuel: number;
  Issue: number;
}

export interface SecurityProperties {
  csrfToken: string;
  _IdentityGuid: string;
  _ID?: any;
  _CreatedDate: Date;
  _CreatedBy?: any;
  _UpdatedDate: Date;
  _UpdatedBy?: any;
  _SiteGuid: string;
  _SiteID: string;
  _Deleted: boolean;
  UserIndex: number;
  UserGuid: string;
  LoginSiteGuid: string;
  UserID: string;
  Token: string;
  LoginSiteID: string;
  ClientCertLogOn: boolean;
  RightsArray: boolean[];
  ASPSessionID?: any;
  ClientDomain?: any;
  ClientUserName?: any;
  ClientIPAddress?: any;
  Workstation?: any;
  ModifyTransactionSecurityRights: object[];
  ViewTransactionSecurityRights: object[];
  EnableChangeTracking: boolean;
  EnableChangeLogging: boolean;
  UseDataDictionary: boolean;
  RowVersion?: any;
}

export interface MaximumLoadAmount {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface MaximumFlushAmount {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface MaximumMeterProvingAmount {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface MaximumReturnsAmount {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface MaximumVehicleWeight {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface MaximumProductTemperature {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface AdministrativeLockDate {
  Value: Date;
  StandardName: string;
  shortDatePattern: string;
  dateSeparator: string;
}

export interface OperationalLockDate {
  Value: Date;
  StandardName: string;
  amDesignator: string;
  pmDesignator: string;
  timePattern: string;
  timeSeparator: string;
  shortDatePattern: string;
  dateSeparator: string;
}

export interface VRURateLimit {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface VRUHourlyLimit {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface VRUDailyLimit {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface VRUYearlyLimit {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface VRUCurrentYearLimit {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface VRURateActual {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface VRUHourlyActual {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface VRUDailyActual {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface VRUYearlyActual {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface VRUCurrentYearActual {
  SIValue: number;
  Units: number;
  ReferenceTemperature: number;
  numberDecimalDigits: number;
  numberGroupSizes: number[];
  numberDecimalSeparator: string;
  numberGroupSeparator: string;
}

export interface Note {
  note: string;
  _IdentityGuid: string;
  _ID: string;
  _CreatedDate: Date;
  _CreatedBy: string;
  _UpdatedDate: Date;
  _UpdatedBy: string;
  _SiteGuid: string;
  _SiteID: string;
  _Deleted: boolean;
  RowVersion?: any;
}

export interface OpeningTime {
  Value: Date;
  amDesignator: string;
  pmDesignator: string;
  timePattern: string;
  timeSeparator: string;
}

export interface ClosingTime {
  Value: Date;
  amDesignator: string;
  pmDesignator: string;
  timePattern: string;
  timeSeparator: string;
}

export interface EndOfDayTime {
  Value: Date;
  amDesignator: string;
  pmDesignator: string;
  timePattern: string;
  timeSeparator: string;
}

export interface OperatingScheduleCollection {
  entityGuid: string;
  type: number;
  day: number;
  holidayDate?: any;
  enabled: boolean;
  openingTime: OpeningTime;
  closingTime: ClosingTime;
  endOfDayEnabled: boolean;
  endOfDayTime: EndOfDayTime;
  _IdentityGuid: string;
  _ID: string;
  _CreatedDate: Date;
  _CreatedBy: string;
  _UpdatedDate: Date;
  _UpdatedBy: string;
  _SiteGuid: string;
  _SiteID: string;
  _Deleted: boolean;
  RowVersion?: any;
}

export interface ProcessVariableCollection {
  ProcessVariableType: number;
  InstanceNumber: number;
  UnitGuid: string;
  UnitType: number;
  OPCConnectionGuid: string;
  _OPCItemID: string;
  _DataType: number;
  ServerUnits: number;
  OPCQuality: number;
  siValue?: number;
  DateTimeStamp: Date;
  siMaximum?: number;
  siMinimum?: number;
  DataTypeEnabled: boolean;
  input: boolean;
  InputEnabled: boolean;
  serverValue?: number;
  ReferenceTemperature: number;
  _URL: string;
  _ProgID: string;
  MessageApplicationStringGuid: string;
  Parent?: any;
  DataChanged: boolean;
  OutputFailed: boolean;
  MessageID: string;
  _IdentityGuid: string;
  _ID: string;
  _CreatedDate: Date;
  _CreatedBy: string;
  _UpdatedDate: Date;
  _UpdatedBy: string;
  _SiteGuid: string;
  _SiteID: string;
  _Deleted: boolean;
  RowVersion?: any;
}

export interface UserData {
  UserData: string[];
}

export interface CurrentSite {
  _LevelDecimalPlaces: number;
  _TemperatureDecimalPlaces: number;
  _DensityDecimalPlaces: number;
  _PressureDecimalPlaces: number;
  _FlowDecimalPlaces: number;
  _VolumeDecimalPlaces: number;
  _MassDecimalPlaces: number;
  _AdditiveVolumeDecimalPlaces: number;
  _AdditiveProfileCycleAmountDecimalPlaces: number;
  _AdditiveProfileRateDecimalPlaces: number;
  _MaximumLoadTime: number;
  _MaximumIdleTime: number;
  _MaximumLoadAmount: MaximumLoadAmount;
  _MaximumFlushAmount: MaximumFlushAmount;
  _MaximumMeterProvingAmount: MaximumMeterProvingAmount;
  _MaximumReturnsAmount: MaximumReturnsAmount;
  _MaximumNumberOfActiveArms: number;
  _DriverTimeoutPeriod: number;
  _DriverWarningPeriod: number;
  _MaximumPrompts: number;
  _AutomaticBOLStartNumber: number;
  _AutomaticBOLEndNumber: number;
  _AutomaticBOLNextNumber: number;
  _SeparateManualBOLNumbering: boolean;
  _ManualBOLStartNumber: number;
  _ManualBOLEndNumber: number;
  _ManualBOLNextNumber: number;
  _TransactionStartNumber: number;
  _TransactionEndNumber: number;
  _TransactionNextNumber: number;
  _OrderStartNumber: number;
  _OrderEndNumber: number;
  _OrderNextNumber: number;
  _InvoiceStartNumber: number;
  _InvoiceEndNumber: number;
  _InvoiceNextNumber: number;
  _EndOfDayWarningPeriod: number;
  _MaximumVehicleWeight: MaximumVehicleWeight;
  _MaximumProductTemperature: MaximumProductTemperature;
  _OpenTransactionWindow: number;
  _AdministrativeLockDate: AdministrativeLockDate;
  _OperationalLockDate: OperationalLockDate;
  _MaximumDaysToRetainLogs: number;
  _VRURateLimit: VRURateLimit;
  _VRUHourlyLimit: VRUHourlyLimit;
  _VRUDailyLimit: VRUDailyLimit;
  _VRUYearlyLimit: VRUYearlyLimit;
  _VRUCurrentYearLimit: VRUCurrentYearLimit;
  _VRURateActual: VRURateActual;
  _VRUHourlyActual: VRUHourlyActual;
  _VRUDailyActual: VRUDailyActual;
  _VRUYearlyActual: VRUYearlyActual;
  _VRUCurrentYearActual: VRUCurrentYearActual;
  _WatchdogPeriod: number;
  _WatchdogCounterStart: number;
  _WatchdogCounterEnd: number;
  NoteGuid: string;
  Note: Note;
  InventoryTransactionAliasID: string;
  AdjustmentTransactionAliasID: string;
  IATAID: string;
  OperatingScheduleCollection: OperatingScheduleCollection[];
  HolidayScheduleCollection: any[];
  ProcessVariableCollection: ProcessVariableCollection[];
  SiteToSiteMapCollection: any[];
  _EnablePasswordHint: boolean;
  _EnablePasswordReset: boolean;
  _AllowUseOfSpecialChars: boolean;
  _EnablePeriodicSyncFlag: boolean;
  _PeriodicSyncIntervalMinutes: number;
  _Number: string;
  _SPLCCode: string;
  _Address1: string;
  _Address2: string;
  _City: string;
  _State: string;
  _Zip: string;
  _Country: string;
  _Phone: string;
  _Fax: string;
  _EmailAddress: string;
  _EmergencyContact: string;
  _EmergencyPhone: string;
  _Enabled: boolean;
  _SiteGroup: boolean;
  _TimeZone: string;
  _IATAGuid: string;
  _inhibitSiteLedgerRollup: boolean;
  _Contact1Name: string;
  _Contact1Address1: string;
  _Contact1Address2: string;
  _Contact1City: string;
  _Contact1State: string;
  _Contact1Zip: string;
  _Contact1Country: string;
  _Contact1PhoneOffice: string;
  _Contact1PhoneMobile: string;
  _Contact1Fax: string;
  _Contact1EmailAddress: string;
  _Contact2Name: string;
  _Contact2Address1: string;
  _Contact2Address2: string;
  _Contact2City: string;
  _Contact2State: string;
  _Contact2Zip: string;
  _Contact2Country: string;
  _Contact2PhoneOffice: string;
  _Contact2PhoneMobile: string;
  _Contact2Fax: string;
  _Contact2EmailAddress: string;
  latitude?: any;
  longitude?: any;
  _LevelUnits: number;
  _TemperatureUnits: number;
  _DensityUnits: number;
  _PressureUnits: number;
  _FlowUnits: number;
  _VolumeUnits: number;
  _MassUnits: number;
  _AdditiveVolumeUnits: number;
  _AdditiveProfileCycleAmountUnits: number;
  _AdditiveProfileRateUnits: number;
  _QuantityDisplayDefault: number;
  _InhibitAccessAfterHours: boolean;
  _InhibitMultipleCardIns: boolean;
  _AccessCardInRequired: boolean;
  _CheckSiteNumber: boolean;
  _PromptForCustomerCard: boolean;
  _PromptForTractorOrTanker: boolean;
  _PromptForFirstTrailer: boolean;
  _PromptForSecondTrailer: boolean;
  _PromptForCompartment: boolean;
  _EnforceDriverEquipmentMatch: boolean;
  _EnableAdditiveAccounting: boolean;
  _UseCompanyEquipmentIdentifiers: boolean;
  _UseLastKnownGoodTankData: boolean;
  _InventoryTransactionAliasGuid: string;
  _AdjustmentTransactionAliasGuid: string;
  _LoadByNet: boolean;
  _PromptForShipmentNumber: boolean;
  _ListEquipment: boolean;
  _DeferStationChanges: boolean;
  _PromptForReturns: boolean;
  _PromptForTruckCard: boolean;
  _StartingShortCardNumber: number;
  _UseShortCardNumber: boolean;
  _ExcessVarianceCount: number;
  _ExcessVarianceTolerance: number;
  _SecondaryStorageFillMethod: number;
  _InhibitBOLWithBrokenBlends: boolean;
  _InhibitBOLWithImproperAdditization: boolean;
  _InhibitOverweightBOL: boolean;
  _ExceptionBOLPrinter: string;
  _EnableAutomaticBOLPrinting: boolean;
  _NumberPrefix: string;
  _EnableDebugLogging: boolean;
  _EnableAuditLogging: boolean;
  _AutomaticallyPrintAlarmsAndEvents: boolean;
  _AlarmAndEventPrinter: string;
  _MailServer: string;
  _MailFrom: string;
  _MailUserName: string;
  _MailPassword: string;
  _MailConnectMode: number;
  _DialupName: string;
  _SCADASystem: string;
  _InhibitTemplateGraphics: boolean;
  _RefreshInterval: number;
  _InhibitEndOfDayOperations: boolean;
  _InhibitEndOfMonthOperations: boolean;
  _InhibitAutomaticPhysicalInventory: boolean;
  _InhibitAutomaticMeterCloseout: boolean;
  _InhibitAutomaticReportGeneration: boolean;
  _InhibitAutomaticAdjustmentDistribution: boolean;
  _InhibitAutomaticCloseout: boolean;
  _InhibitTankScan: boolean;
  _ReportDirectory: string;
  _ManageReports: boolean;
  _ManagedReportDirectory: string;
  _EnforceSingleOwner: boolean;
  _InhibitBOLSummaryAutoPopulate: boolean;
  _InhibitOrderSummaryAutoPopulate: boolean;
  _InhibitSupplyOrderSummaryAutoPopulate: boolean;
  _ExportArchiveDir: string;
  _ImportArchiveDir: string;
  _GroupLedgerByID: boolean;
  _VRURateLimitEnabled: boolean;
  _VRUHourlyLimitEnabled: boolean;
  _VRUDailyLimitEnabled: boolean;
  _VRUYearlyLimitEnabled: boolean;
  _VRUCurrentYearLimitEnabled: boolean;
  _WatchdogMode: number;
  _NumberGroupSizesType: number;
  _NumberDecimalSeparator: string;
  _NumberGroupSeparator: string;
  _ListSeparator: string;
  _TimePattern: string;
  _TimeSeparator: string;
  _AMSymbol: string;
  _PMSymbol: string;
  _ShortDatePattern: string;
  _DateSeparator: string;
  _LongDatePattern: string;
  _TwoDigitCalendarEndYear: number;
  minTimeAllowedToChangePwd: number;
  minPwdCharacterLength: number;
  pwdExpirationInDays: number;
  pwdLockoutThreshold: number;
  pwdHistoryCount: number;
  checkForPreviousPwd: boolean;
  StrongPwdUse: number;
  applyToAllSiteMembers: boolean;
  inactivityDisablePeriod: number;
  disableArchivePeriod: number;
  useTankReconciliation: boolean;
  _MeterReconciliationToleranceIsPercent: boolean;
  _MeterReconciliationReportName: string;
  _TranslatedHelpURL: string;
  UserData: UserData;
  _IdentityGuid: string;
  _ID: string;
  _CreatedDate: Date;
  _CreatedBy: string;
  _UpdatedDate: Date;
  _UpdatedBy: string;
  _SiteGuid: string;
  _SiteID: string;
  _Deleted: boolean;
  RowVersion?: any;
}

export interface Transaction {
  _TransTypeID: number;
  AssociatedReport: string;
  AssociatedPreloadReport: string;
  ExcludedProductCollection: any[];
  LookupDefaultStatusIndex: number;
  GroupTransactionAliasMapCollection: any[];
  AssociatedAlias: string;
  _LevelDecimalPlaces: number;
  _TemperatureDecimalPlaces: number;
  _DensityDecimalPlaces: number;
  _PressureDecimalPlaces: number;
  _FlowDecimalPlaces: number;
  _VolumeDecimalPlaces: number;
  _MassDecimalPlaces: number;
  _AdditiveVolumeDecimalPlaces: number;
  _IncludeInDispatch: boolean;
  _MeterCloseout: boolean;
  _BulkShipment: boolean;
  _DistributedImpact: boolean;
  _MultipleLineItems: boolean;
  _LineItemEditControl: boolean;
  _MultipleWeightReadings: boolean;
  _LimitSelectionsBasedOnHierarchy: boolean;
  _WeightReadingEditControl: boolean;
  multipleTransportLineItems: boolean;
  _AssociatedTransactionAliasGuid: string;
  _DestinationEquipmentTypes: number[];
  _SourceEquipmentTypes: number[];
  _UseComboBoxControls: boolean;
  _showCompanyName: number;
  enableTotalQtyExceededWarning: boolean;
  enableTotalValueExceededWarning: boolean;
  enableQtyToleranceExceededWarning: boolean;
  enableValueToleranceExceededWarning: boolean;
  associatedAliases: any[];
  assignedStatuses: any[];
  aggregateAssociatedTransactions: boolean;
  _LevelUnits: number;
  _TemperatureUnits: number;
  _DensityUnits: number;
  _PressureUnits: number;
  _FlowUnits: number;
  _VolumeUnits: number;
  _MassUnits: number;
  _AdditiveVolumeUnits: number;
  _MasterRecordGuid: string;
  _AssignedToSiteGuid: string;
  _AssignedFromSiteGuid: string;
  _AssignedFromSiteId: string;
  ENTITY_TYPE_ID: string;
  _IdentityGuid: string;
  _ID: string;
  _CreatedDate: Date;
  _CreatedBy: string;
  _UpdatedDate: Date;
  _UpdatedBy: string;
  _SiteGuid: string;
  _SiteID: string;
  _Deleted: boolean;
  UserDataFieldCollection: any[];
  LineItemUserDataFieldCollection: any[];
  TransactionFieldCollection: any[];
  LineItemFieldCollection: any[];
  WeightReadingFieldCollection: any[];
  TransportLineItemFieldCollection: any[];
  NoteFieldCollection: any[];
  ExportResultDetailFieldCollection: any[];
  DispatchUserDataFields: any[];
  DispatchLineItemUserDataFields: any[];
  DispatchTransactionFields: any[];
  DispatchLineItemFields: any[];
  DispatchWeightReadingFields: any[];
  DispatchTransportLineItemFields: any[];
  DispatchNoteFields: any[];
  DispatchExportResultDetailFields: any[];
  EnableAutoCompleteControls: boolean;
  PermitNonReferenceData: boolean;
  RowVersion?: any;
}

export interface LoginResponse {
  LoginSuccess: boolean;
  MustChangePassword: boolean;
  DaysUntilPasswordExpires: number;
  SecurityProperties: SecurityProperties;
  CurrentSite: CurrentSite;
  Transactions: Transaction[];
}
