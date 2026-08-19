/* {CheckPoint: CREATING TRACKING TABLE for tblPointAccessGroupToUserGroup } */

/****** Object:  Table [track].[tblPointAccessGroupToUserGroup]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPointAccessGroupToUserGroup]
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
	[PK_PointAccessGroupToUserGroupGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPointAccessGroupToUserGroup_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToUserGroup_PK_PointAccessGroupToUserGroupGuid] ON [track].[tblPointAccessGroupToUserGroup]
(
    [PK_PointAccessGroupToUserGroupGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToUserGroup_InsertedRowVersion] ON [track].[tblPointAccessGroupToUserGroup]
(
    [InsertedRowVersion] ASC,
    [PK_PointAccessGroupToUserGroupGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToUserGroup_UpdatedRowVersion] ON [track].[tblPointAccessGroupToUserGroup]
(
    [UpdatedRowVersion] ASC,
    [PK_PointAccessGroupToUserGroupGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToUserGroup_DeletedRowVersion] ON [track].[tblPointAccessGroupToUserGroup]
(
    [DeletedRowVersion] ASC,
    [PK_PointAccessGroupToUserGroupGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPointAccessGroupToUserGroup_PK_PointAccessGroupToUserGroupGuid_Sync] ON [track].[tblPointAccessGroupToUserGroup]
(
	[PK_PointAccessGroupToUserGroupGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPointAccessGroupToUserGroup_DeletedRowVersionUpdate_ForSync
   ON track.tblPointAccessGroupToUserGroup
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
        FROM track.tblPointAccessGroupToUserGroup t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END