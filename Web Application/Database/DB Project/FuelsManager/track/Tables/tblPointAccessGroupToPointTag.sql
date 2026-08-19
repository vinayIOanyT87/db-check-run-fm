CREATE TABLE [track].[tblPointAccessGroupToPointTag]
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
	[PK_PointAccessGroupToPointTagGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointAccessGroupToPointTag_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointTag_PK_PointAccessGroupToPointTagGuid] ON [track].[tblPointAccessGroupToPointTag]
(
    [PK_PointAccessGroupToPointTagGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointTag_InsertedRowVersion] ON [track].[tblPointAccessGroupToPointTag]
(
    [InsertedRowVersion] ASC,
    [PK_PointAccessGroupToPointTagGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointTag_UpdatedRowVersion] ON [track].[tblPointAccessGroupToPointTag]
(
    [UpdatedRowVersion] ASC,
    [PK_PointAccessGroupToPointTagGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointTag_DeletedRowVersion] ON [track].[tblPointAccessGroupToPointTag]
(
    [DeletedRowVersion] ASC,
    [PK_PointAccessGroupToPointTagGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToPointTag_PK_PointAccessGroupToTagGuid_Sync] ON [track].[tblPointAccessGroupToPointTag]
(
	[PK_PointAccessGroupToPointTagGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
