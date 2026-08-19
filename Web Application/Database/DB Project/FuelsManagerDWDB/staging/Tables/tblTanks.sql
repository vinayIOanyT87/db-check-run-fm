CREATE TABLE [staging].[tblTanks](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[TankID] [nvarchar](50) NULL,
	[TankGuid] [uniqueidentifier] NULL,
	[TankKey] [nvarchar](50) NULL,
	[SiteGuid] [uniqueidentifier] NULL,
	[SiteKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,
	[LookupVesselTypeIndex] [int] NULL,
	[VesselTypeName] [nvarchar](100) NULL,
	[ManagerCompanyGuid] [uniqueidentifier] NULL,
	[ManagerCompanyKey] [nvarchar](50) NULL,
	[ProductGuid] [uniqueidentifier] NULL,
	[ProductKey] [nvarchar](50) NULL,
	[HiddenDate] [datetimeoffset](7) NULL,
	[AssetTrackingDeviceGuid] [uniqueidentifier] NULL,
	[AssetTrackingDeviceKey] [nvarchar](50) NULL,
	[LookupDeviceTankTypeIndex] [int] NULL,
	[DeviceTankTypeName] [nvarchar](100) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[TankConfigurationNumber] [int] NULL,
	[Zoom] [int] NULL,
	[OwnerCompanyGuid] [uniqueidentifier] NULL,
	[OwnerCompanyKey] [nvarchar](50) NULL,

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
	[_RowVersion] [timestamp] NOT NULL
 CONSTRAINT [PK_tblTanks] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblTanks] ADD  DEFAULT ((0)) FOR [IsRecordDeleted]
GO
ALTER TABLE [staging].[tblTanks] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblTanks] ADD  DEFAULT ((0)) FOR [IgnoreRecord]