CREATE TABLE [staging].[tblEquipmentTypes](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[EquipmentTypeGuid] [uniqueidentifier] NULL,
	[EquipmentTypeKey] [nvarchar](50) NULL,
	[EqTypeName] [nvarchar](50) NULL,
	[EqTypeDescription] [nvarchar](50) NULL,
	[Capacity] [float] NULL,
	[SafeFill] [float] NULL,
	[Make] [nvarchar](20) NULL,
	[Model] [nvarchar](32) NULL,
	[Year] [smallint] NULL,
	[DeleteFlag] [bit] NULL,
	[IssPt] [nvarchar](20) NULL,
	[MultiCompartment] [bit] NULL,	
	[SiteGuid] [uniqueidentifier] NULL,
	[SiteKey] [nvarchar](50) NULL,
	[SiteSKey] [uniqueidentifier] NULL,
	[LookupEquipmentTypeIndex] [int] NULL,
	[LookupEquipmentTypeName] [nvarchar](100) NULL,
	[ProductGuid] [uniqueidentifier] NULL,
	[ProductKey] [nvarchar](50) NULL,
	[ProductSKey] [int] NULL,
	[CustomerDesignator] [nvarchar](128) NULL,
	[ServiceTime] [float] NULL,
	[VolumeUnits] [int] NULL,
	[VolumeDecimalPlaces] [smallint] NULL,
	[MassUnits] [int] NULL,
	[MassDecimalPlaces] [smallint] NULL,
	[WingToWingToleranceType] [smallint] NULL,
	[WingToWingToleranceValue] [float] NULL,
	[TankToTankToleranceType] [smallint] NULL,
	[TankToTankToleranceValue] [float] NULL,
	[FuelServiceToleranceType] [smallint] NULL,
	[FuelServiceToleranceValue] [float] NULL,
	[FuelServiceToleranceMaxType] [smallint] NULL,
	[FuelServiceToleranceMaxValue] [float] NULL,
	[AllowFuelingByWeight] [bit] NULL,
	[LookupCompanyRoleIndex] [int] NULL,
	[_ClusterIdx] [bigint] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDate] [datetimeoffset](7) NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[CombinedUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NOT NULL,
	[IsRecordAddedByETL] [bit] NOT NULL,
	[IgnoreRecord] [bit] NOT NULL,
	[CDCSKey] [int] NULL,
	[SourceRowVersion] [bigint] NULL,
	[CDCRowVersion] [bigint] NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblEquipmentTypes] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblEquipmentTypes] ADD  DEFAULT ((0)) FOR [IsRecordDeleted]
GO
ALTER TABLE [staging].[tblEquipmentTypes] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblEquipmentTypes] ADD  DEFAULT ((0)) FOR [IgnoreRecord]