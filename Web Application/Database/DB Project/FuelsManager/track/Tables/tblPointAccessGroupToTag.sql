/* {CheckPoint: CREATING TRACKING TABLE for tblPointAccessGroupToTag } */

/****** Object:  Table [track].[tblPointAccessGroupToTag]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPointAccessGroupToTag]
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
	[PK_PointAccessGroupToTagGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointAccessGroupToTag_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToTag_PK_PointAccessGroupToTagGuid] ON [track].[tblPointAccessGroupToTag]
(
    [PK_PointAccessGroupToTagGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToTag_InsertedRowVersion] ON [track].[tblPointAccessGroupToTag]
(
    [InsertedRowVersion] ASC,
    [PK_PointAccessGroupToTagGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToTag_UpdatedRowVersion] ON [track].[tblPointAccessGroupToTag]
(
    [UpdatedRowVersion] ASC,
    [PK_PointAccessGroupToTagGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToTag_DeletedRowVersion] ON [track].[tblPointAccessGroupToTag]
(
    [DeletedRowVersion] ASC,
    [PK_PointAccessGroupToTagGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToTag_PK_PointAccessGroupToTagGuid_Sync] ON [track].[tblPointAccessGroupToTag]
(
	[PK_PointAccessGroupToTagGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPointAccessGroupToTag_DeletedRowVersionUpdate_ForSync
   ON track.tblPointAccessGroupToTag
   AFTER UPDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
    IF ( UPDATE( DeletedDate ) )
    BEGIN
        UPDATE t
            SET DeletedRowVersion = convert(varbinary(8), i._RowVersion)
        FROM track.tblPointAccessGroupToTag t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END