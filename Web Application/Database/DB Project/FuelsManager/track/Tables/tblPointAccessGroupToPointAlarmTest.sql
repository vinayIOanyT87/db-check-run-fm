CREATE TABLE [track].[tblPointAccessGroupToPointAlarmTest]
(
	[ChangeIndex] [bigint] IDENTITY(1,1) NOT NULL,
	[InsertedDate] [datetimeoffset](7) NOT NULL,
	[InsertedContext] [varbinary](128) NULL,
	[InsertedRowVersion] [varbinary](8) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[UpdatedContext] [varbinary](128) NULL,
	[UpdatedRowVersion] [varbinary](8) NULL,
	[DeletedDate] [datetimeoffset](7) NULL,
	[DeletedContext] [varbinary](128) NULL,
	[DeletedRowVersion] [varbinary](8) NULL,
	[CurrentSiteGuid] [uniqueidentifier] NULL,
	[PreviousSiteGuid] [uniqueidentifier] NULL,
	[PK_PointAccessGroupToPointAlarmTestGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointAccessGroupToPointAlarmTest_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointAlarmTest_PK_PointAccessGroupToPointAlarmTestGuid] ON [track].[tblPointAccessGroupToPointAlarmTest]
(
    [PK_PointAccessGroupToPointAlarmTestGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointAlarmTest_InsertedRowVersion] ON [track].[tblPointAccessGroupToPointAlarmTest]
(
    [InsertedRowVersion] ASC,
    [PK_PointAccessGroupToPointAlarmTestGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointAlarmTest_UpdatedRowVersion] ON [track].[tblPointAccessGroupToPointAlarmTest]
(
    [UpdatedRowVersion] ASC,
    [PK_PointAccessGroupToPointAlarmTestGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointAlarmTest_DeletedRowVersion] ON [track].[tblPointAccessGroupToPointAlarmTest]
(
    [DeletedRowVersion] ASC,
    [PK_PointAccessGroupToPointAlarmTestGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointAlarmTest_PK_PointAccessGroupToAlarmTestGuid_Sync] ON [track].[tblPointAccessGroupToPointAlarmTest]
(
	[PK_PointAccessGroupToPointAlarmTestGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
