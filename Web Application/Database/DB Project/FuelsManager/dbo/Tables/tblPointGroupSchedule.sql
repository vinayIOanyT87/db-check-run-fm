CREATE TABLE [dbo].[tblPointGroupSchedule](
	[PointGroupScheduleGuid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_tblPointGroupSchedule_GUID]  DEFAULT (newid()),
	[PointGroupGuid] [uniqueidentifier] NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[CronSchedule] nvarchar(50) NULL,
	[StartSchedule] datetime NULL,
	[EndSchedule] nvarchar(50) NULL,
	[Printer] nvarchar(50) NULL,
	[EmailTo] nvarchar(150) NULL,
	[Layout] int NOT NULL DEFAULT (1),
	[ExportFileFormat] int NOT NULL DEFAULT (1),
	[CreateNewExportFile] bit NOT NULL DEFAULT (0),
	[FitToPage] bit NOT NULL DEFAULT (0),
	[CreatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblPointGroupSchedule_CreatedDate]  DEFAULT (sysdatetimeoffset()),
	[CreatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblPointGroupSchedule_CreatedBy]  DEFAULT (''),
	[UpdatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblPointGroupSchedule_UpdatedDate]  DEFAULT (sysdatetimeoffset()),
	[UpdatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblPointGroupSchedule_UpdatedBy]  DEFAULT (''),
	[_RowVersion] [timestamp] NOT NULL,
	[_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblPointGroupSchedule_GUID] PRIMARY KEY NONCLUSTERED 
(
	[PointGroupScheduleGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblPointGroupSchedule]  WITH NOCHECK ADD  CONSTRAINT [FK_tblPointGroupSchedule_SiteGuid] FOREIGN KEY([SiteGuid])
REFERENCES [dbo].[tblSites] ([SiteGuid])
GO

ALTER TABLE [dbo].[tblPointGroupSchedule]  WITH NOCHECK ADD  CONSTRAINT [FK_tblPointGroupSchedule_PointGroupGuid] FOREIGN KEY([PointGroupGuid])
REFERENCES [dbo].[tblPointGroup] ([PointGroupGuid])
GO

ALTER TABLE [dbo].[tblPointGroupSchedule] CHECK CONSTRAINT [FK_tblPointGroupSchedule_SiteGuid]
GO

ALTER TABLE [dbo].[tblPointGroupSchedule] CHECK CONSTRAINT [FK_tblPointGroupSchedule_PointGroupGuid]
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblPointGroupSchedule_ClusterIdx] 
	ON [dbo].[tblPointGroupSchedule]([_ClusterIdx]);
GO
