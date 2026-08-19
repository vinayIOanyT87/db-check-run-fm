/*
	DROP TABLE [erv].[tblTempTransactionAliasRecordVersioningFlag]
*/
CREATE TABLE [erv].[tblTempTransactionAliasRecordVersioningFlag] (
    [TransactionAliasRVFlagGuid]                    UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTempTransactionAliasRecordVersioningFlag_GUID] DEFAULT (newid()) NOT NULL,
    [TransactionAliasGuid]                          UNIQUEIDENTIFIER   NULL,
    [SiteGuid]                                      UNIQUEIDENTIFIER   NULL,
    [AdditiveProfileCycleAmountUnitIndex_RVFlag]    BIT                NULL,
    [AdditiveProfileRateUnitIndex_RVFlag]           BIT                NULL,
    [AdditiveVolumeDecimalPlaces_RVFlag]            BIT                NULL,
    [AdditiveVolumeUnitIndex_RVFlag]                BIT                NULL,
    [AggregateAssocTrans_RVFlag]                    BIT                NULL,
    [AliasName_RVFlag]                              BIT                NULL,
    [AssociatedPreloadReport_RVFlag]                BIT                NULL,
    [AssociatedReport_RVFlag]                       BIT                NULL,
    [AssociatedTransactionAliasGuid_RVFlag]         BIT                NULL,
    [BulkShipment_RVFlag]                           BIT                NULL,
    [DensityDecimalPlaces_RVFlag]                   BIT                NULL,
    [DensityUnitIndex_RVFlag]                       BIT                NULL,
    [DestinationEquipmentTypes1_RVFlag]             BIT                NULL,
    [DestinationEquipmentTypes2_RVFlag]             BIT                NULL,
    [DestinationEquipmentTypes3_RVFlag]             BIT                NULL,
    [DistributedImpact_RVFlag]                      BIT                NULL,
	[EnableAutoCompleteControls_RVFlag]             BIT                NULL,
    [EnableQuantityToleranceExceededWarning_RVFlag] BIT                NULL,
    [EnableTotalQuantityExceededWarning_RVFlag]     BIT                NULL,
    [EnableTotalValueExceededWarning_RVFlag]        BIT                NULL,
    [EnableValueToleranceExceededWarning_RVFlag]    BIT                NULL,
    [FlowDecimalPlaces_RVFlag]                      BIT                NULL,
    [FlowUnitIndex_RVFlag]                          BIT                NULL,
    [IncludeInDispatch_RVFlag]                      BIT                NULL,
    [LevelDecimalPlaces_RVFlag]                     BIT                NULL,
    [LevelUnitIndex_RVFlag]                         BIT                NULL,
    [LimitSelectionsBasedOnHierarchy_RVFlag]        BIT                NULL,
    [LineItemEditControl_RVFlag]                    BIT                NULL,
    [LookupDefaultStatusIndex_RVFlag]               BIT                NULL,
    [LookupTransTypeIndex_RVFlag]                   BIT                NULL,
    [MassDecimalPlaces_RVFlag]                      BIT                NULL,
    [MassUnitIndex_RVFlag]                          BIT                NULL,
    [MeterCloseout_RVFlag]                          BIT                NULL,
    [MultipleLineItems_RVFlag]                      BIT                NULL,
    [MultipleTransportLineItems_RVFlag]             BIT                NULL,
    [MultipleWeightReadings_RVFlag]                 BIT                NULL,
	[PermitNonReferenceData_RVFlag]                 BIT                NULL,
    [PressureDecimalPlaces_RVFlag]                  BIT                NULL,
    [PressureUnitIndex_RVFlag]                      BIT                NULL,
    [ShowCompanyName_RVFlag]                        BIT                NULL,
    [SourceEquipmentTypes1_RVFlag]                  BIT                NULL,
    [SourceEquipmentTypes2_RVFlag]                  BIT                NULL,
    [SourceEquipmentTypes3_RVFlag]                  BIT                NULL,
    [TemperatureDecimalPlaces_RVFlag]               BIT                NULL,
    [TemperatureUnitIndex_RVFlag]                   BIT                NULL,
    [UpdatedBy_RVFlag]                              BIT                NULL,
    [UpdatedDate_RVFlag]                            BIT                NULL,
    [UseComboBoxControls_RVFlag]                    BIT                NULL,
    [VolumeDecimalPlaces_RVFlag]                    BIT                NULL,
    [VolumeUnitIndex_RVFlag]                        BIT                NULL,
    [WeightReadingEditControl_RVFlag]               BIT                NULL,
    [_CallingReferenceGuid]                         UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempTransactionAliasRecordVersioningFlag_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                                     [dbo].[udtUserID]  CONSTRAINT [DF_tblTempTransactionAliasRecordVersioningFlag_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempTransactionAliasRecordVersioningFlag_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                                     [dbo].[udtUserID]  CONSTRAINT [DF_tblTempTransactionAliasRecordVersioningFlag_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]                                   ROWVERSION         NOT NULL,
    [_ClusterIdx]                                   BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTempTransactionAliasRecordVersioningFlag] PRIMARY KEY NONCLUSTERED ([TransactionAliasRVFlagGuid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblTempTransactionAliasRecordVersioningFlag_TransactionAliasGuid]
    ON [erv].[tblTempTransactionAliasRecordVersioningFlag]([TransactionAliasGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTempTransactionAliasRecordVersioningFlag_CallingReferenceGuid]
    ON [erv].[tblTempTransactionAliasRecordVersioningFlag]([_CallingReferenceGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTempTransactionAliasRecordVersioningFlag_ClusterIdx]
    ON [erv].[tblTempTransactionAliasRecordVersioningFlag]([_ClusterIdx] ASC);

GO
