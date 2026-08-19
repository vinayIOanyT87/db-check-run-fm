CREATE TABLE [staging].[tblOwnerCloseout](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[OwnerCloseoutGuid] [uniqueidentifier] NULL,
	[OwnerCloseoutKey] [nvarchar](50) NULL,
	[ManagerName] [nvarchar](100) NOT NULL,
	[ProductName] [nvarchar](30) NOT NULL,
	[CloseoutDate] [date] NOT NULL,
	[OwnerName] [nvarchar](100) NULL,
	[GrossBookInventory] [float] NULL,
	[NetBookInventory] [float] NULL,
	[GrossBookPrice] [float] NULL,
	[NetBookPrice] [float] NULL,
	[TransVersion] [bigint] NULL,
	[MassBookInventory] [float] NULL,
	[MassBookPrice] [float] NULL,	
	[SiteGuid] [uniqueidentifier] NULL,
	[SiteKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,
	[Site] [nvarchar](30) NULL,
	[ManagerCompanyGuid] [uniqueidentifier] NULL,
	[ManagerCompanyKey] [nvarchar](50) NULL,
	[ManagerCompanySKey] [int] NULL,
	[OwnerCompanyGuid] [uniqueidentifier] NULL,
	[OwnerCompanyKey] [nvarchar](50) NULL,
	[OwnerCompanySKey] [int] NULL,
	[ProductGuid] [uniqueidentifier] NULL,
	[ProductKey] [nvarchar](50) NULL,
	[ProductSKey] [int] NULL,
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
 CONSTRAINT [PK_tblOwnerCloseout] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblOwnerCloseout] ADD  DEFAULT ((0)) FOR [IsRecordDeleted]
GO
ALTER TABLE [staging].[tblOwnerCloseout] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblOwnerCloseout] ADD  DEFAULT ((0)) FOR [IgnoreRecord]