CREATE TABLE [fmcdc].[tblLoadArms](
	[LoadArmsSKey] [int] IDENTITY(1,1) NOT NULL,
	[LoadRackText] [nvarchar](9) NULL, 
	[Enabled] [bit] NULL, 
	[SwingArm] [bit] NULL, 
	[BayAArmNumber] [int] NULL, 
	[BayBArmNumber] [int] NULL, 
	[CreatedDate] [datetimeoffset](7) NULL, 
	[CreatedBy] [udtUserID] NULL, 
	[UpdatedDate] [datetimeoffset](7) NULL, 
	[UpdatedBy] [udtUserID] NULL, 
	[LoadArmGuid] [uniqueidentifier] NULL, 
	[SourceRowVersion] [bigint] NULL, 
	[LookupPresetTypeIndex] [int] NULL, 
	[BayAStationGuid] [uniqueidentifier] NULL, 
	[BayBStationGuid] [uniqueidentifier] NULL, 
	[_ClusterIdx] [bigint] NULL, 
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NULL,
	[_RowVersion] [timestamp] NOT NULL,
CONSTRAINT [PK_tblLoadArms] PRIMARY KEY CLUSTERED
(
	[LoadArmsSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO