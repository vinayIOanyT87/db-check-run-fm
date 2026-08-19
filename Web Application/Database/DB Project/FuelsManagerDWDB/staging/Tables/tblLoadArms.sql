CREATE TABLE [staging].[tblLoadArms]
(
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[LoadRackText] [nvarchar](9) NULL,
	[Enabled] [bit] NULL,
	[SwingArm] [bit] NULL,
	[BayAArmNumber] [int] NULL,
	[BayBArmNumber] [int] NULL,
	[LoadArmGuid] [uniqueidentifier] NULL,
	[LoadArmKey] [nvarchar](50) NULL,
	[LookupPresetTypeIndex] [int] NULL,
	[BayAStationGuid] [uniqueidentifier] NULL,
	[BayAStationKey] [nvarchar](50) NULL,
	[BayAStationSKey] [int] NULL,
	[BayBStationGuid] [uniqueidentifier] NULL,
	[BayBStationKey] [nvarchar](50) NULL,
	[BayBStationSKey] [int] NULL,

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
 CONSTRAINT [PK_tblLoadArms] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblLoadArms] ADD  DEFAULT ((0)) FOR [IsRecordDeleted]
GO
ALTER TABLE [staging].[tblLoadArms] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblLoadArms] ADD  DEFAULT ((0)) FOR [IgnoreRecord]

