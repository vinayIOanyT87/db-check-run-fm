CREATE TABLE [fmcdc].[tblTanks](
	[TanksSKey] [int] IDENTITY(1,1) NOT NULL,
	[TankID] [nvarchar](50) NULL, 
	[CreatedDate] [datetimeoffset](7) NULL, 
	[CreatedBy] [udtUserID] NULL, 
	[UpdatedDate] [datetimeoffset](7) NULL, 
	[UpdatedBy] [udtUserID] NULL, 
	[TankGuid] [uniqueidentifier] NULL, 
	[SourceRowVersion] [bigint] NULL, 
	[SiteGuid] [uniqueidentifier] NULL, 
	[LookupVesselTypeIndex] [int] NULL, 
	[ManagerCompanyGuid] [uniqueidentifier] NULL, 
	[ProductGuid] [uniqueidentifier] NULL, 
	[HiddenDate] [datetimeoffset](7) NULL, 
	[_ClusterIdx] [bigint] NULL, 
	[AssetTrackingDeviceGuid] [uniqueidentifier] NULL, 
	[LookupDeviceTankTypeIndex] [int] NULL, 
	[Latitude] [float] NULL, 
	[Longitude] [float] NULL, 
	[TankConfigurationNumber] [int] NULL, 
	[Zoom] [int] NULL, 
	[OwnerCompanyGuid] [uniqueidentifier] NULL, 
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NULL,
	[_RowVersion] [timestamp] NOT NULL,
CONSTRAINT [PK_tblTanks] PRIMARY KEY CLUSTERED
(
	[TanksSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO