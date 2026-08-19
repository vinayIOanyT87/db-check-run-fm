/*
	DROP TABLE [erv].[tblTempProductRecordVersioningFlag] 
*/
CREATE TABLE [erv].[tblTempProductRecordVersioningFlag] (
    [ProductRVFlagGuid]                       UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTempProductRecordVersioningFlag_GUID] DEFAULT (newid()) NOT NULL,
    [ProductGuid]                             UNIQUEIDENTIFIER   NULL,
    [SiteGuid]                                UNIQUEIDENTIFIER   NULL,
	[ApplyDensityLimits_RVFlag]               BIT                NULL,
    [ApplyStandardDensity_RVFlag]             BIT                NULL,
    [ApplyTemperatureLimits_RVFlag]           BIT                NULL,
    [ApplyVolumeCorrection_RVFlag]            BIT                NULL,
	[AutomaticCloseout_RVFlag]                BIT                NULL,
    [AviationFuelFlag_RVFlag]                 BIT                NULL,
    [Bonded_RVFlag]                           BIT                NULL,
    [Capitalize_RVFlag]                       BIT                NULL,
    [ComponentTolerance_RVFlag]               BIT                NULL,
    [ContaminationPromptLoadRackText_RVFlag]  BIT                NULL,
    [DensityDeadband_RVFlag]                  BIT                NULL,
    [DensityDecimalPlaces_RVFlag]             BIT                NULL,
    [DensityHighLimit_RVFlag]                 BIT                NULL,
    [DensityLowLimit_RVFlag]                  BIT                NULL,
    [DensityUnitIndex_RVFlag]                 BIT                NULL,
    [Description_RVFlag]                      BIT                NULL,
	[DielectricTolerance_RVFlag]              BIT                NULL,
    [FlowDecimalPlaces_RVFlag]                BIT                NULL,
    [FlowUnitIndex_RVFlag]                    BIT                NULL,
    [GenericType_RVFlag]                      BIT                NULL,
    [GroundFuel_RVFlag]                       BIT                NULL,
    [HazardousMaterial_RVFlag]                BIT                NULL,
	[HiddenDate_RVFlag]                       BIT                NULL,
    [InhibitAccounting_RVFlag]                BIT                NULL,
    [LevelDecimalPlaces_RVFlag]               BIT                NULL,
    [LevelUnitIndex_RVFlag]                   BIT                NULL,
    [LoadByWeight_RVFlag]                     BIT                NULL,
    [LoadRackDisplayText_RVFlag]              BIT                NULL,
    [LockedOut_RVFlag]                        BIT                NULL,
    [LockedOutDate_RVFlag]                    BIT                NULL,
    [LockedOutReason_RVFlag]                  BIT                NULL,
    [LookupProductTypeIndex_RVFlag]           BIT                NULL,
    [LowStockWarning_RVFlag]                  BIT                NULL,
    [MassDecimalPlaces_RVFlag]                BIT                NULL,
    [MassPackageSize_RVFlag]                  BIT                NULL,
    [MassUnitIndex_RVFlag]                    BIT                NULL,
    [OctaneNumber_RVFlag]                     BIT                NULL,
	[PatternColor_RVFlag]                     BIT                NULL,
	[PatternNumber_RVFlag]                    BIT                NULL,
    [PIDXCode_RVFlag]                         BIT                NULL,
	[PIDXFamilyCode_RVFlag]                   BIT                NULL,
    [PressureDecimalPlaces_RVFlag]            BIT                NULL,
    [PressureUnitIndex_RVFlag]                BIT                NULL,
    [Price_RVFlag]                            BIT                NULL,
    [ProductCode_RVFlag]                      BIT                NULL,
    [ProductColor_RVFlag]                     BIT                NULL,
    [ProductID_RVFlag]                        BIT                NULL,
    [RegulatoryClass_RVFlag]                  BIT                NULL,
    [ReidVaporPressure_RVFlag]                BIT                NULL,
	[StandardDensity_RVFlag]                  BIT				 NULL,
    [StockResetDate_RVFlag]                   BIT                NULL,
    [StockTrack_RVFlag]                       BIT                NULL,
    [TaxCode_RVFlag]                          BIT                NULL,
    [TemperatureDeadband_RVFlag]              BIT                NULL,
    [TemperatureDecimalPlaces_RVFlag]         BIT                NULL,
    [TemperatureHighLimit_RVFlag]             BIT                NULL,
    [TemperatureHiHiLimit_RVFlag]             BIT                NULL,
    [TemperatureLoLoLimit_RVFlag]             BIT                NULL,
    [TemperatureLowLimit_RVFlag]              BIT                NULL,
    [TemperatureUnitIndex_RVFlag]             BIT                NULL,
    [TrackingProductGuid_RVFlag]              BIT                NULL,
    [UpdatedBy_RVFlag]                        BIT                NULL,
    [UpdatedDate_RVFlag]                      BIT                NULL,
    [UserData1_RVFlag]                        BIT                NULL,
    [UserData2_RVFlag]                        BIT                NULL,
    [UserData3_RVFlag]                        BIT                NULL,
    [UserData4_RVFlag]                        BIT                NULL,
    [UserData5_RVFlag]                        BIT                NULL,
    [UserData6_RVFlag]                        BIT                NULL,
    [UserData7_RVFlag]                        BIT                NULL,
    [UserData8_RVFlag]                        BIT                NULL,
    [VaporRecovery_RVFlag]                    BIT                NULL,
    [VarianceTolerance_RVFlag]                BIT                NULL,
    [VcfModuleSettings_RVFlag]                BIT                NULL,
    [VolumeDecimalPlaces_RVFlag]              BIT                NULL,
    [VolumePackageSize_RVFlag]                BIT                NULL,
    [VolumeUnitIndex_RVFlag]                  BIT                NULL,
    [_CallingReferenceGuid]                   UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                             DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempProductRecordVersioningFlag_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                               [dbo].[udtUserID]  CONSTRAINT [DF_tblTempProductRecordVersioningFlag_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                             DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempProductRecordVersioningFlag_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                               [dbo].[udtUserID]  CONSTRAINT [DF_tblTempProductRecordVersioningFlag_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]                             ROWVERSION         NOT NULL,		
    [_ClusterIdx]                             BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTempProductRecordVersioningFlag] PRIMARY KEY NONCLUSTERED ([ProductRVFlagGuid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblTempProductRecordVersioningFlag_ProductGuid]
    ON [erv].[tblTempProductRecordVersioningFlag]([ProductGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTempProductRecordVersioningFlag_CallingReferenceGuid]
    ON [erv].[tblTempProductRecordVersioningFlag]([_CallingReferenceGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTempProductRecordVersioningFlag_ClusterIdx]
    ON [erv].[tblTempProductRecordVersioningFlag]([_ClusterIdx] ASC);

GO
