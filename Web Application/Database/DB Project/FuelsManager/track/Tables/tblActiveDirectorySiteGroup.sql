/* {CheckPoint: CREATING TRACKING TABLE for tblActiveDirectorySiteGroup } */

/****** Object:  Table [track].[tblActiveDirectorySiteGroup]   Script Date: 8/28/2012 3:24:01 PM ******/
/* {CheckPoint: CREATING TRACKING TABLE for tblActiveDirectorySiteGroup } */

/****** Object:  Table [track].[tblActiveDirectorySiteGroup]   Script Date: 8/28/2012 3:24:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectorySiteGroup]') AND type in (N'U'))
--BEGIN
--Creating Sync Tracking Table for tblActiveDirectorySiteGroup
CREATE TABLE [track].[tblActiveDirectorySiteGroup]
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
	[PK_ActiveDirectorySiteGroupGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblActiveDirectorySiteGroup_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
--END
GO

CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectorySiteGroup_PK_ActiveDirectorySiteGroupGuid] ON [track].[tblActiveDirectorySiteGroup]
(
    [PK_ActiveDirectorySiteGroupGuid],
    [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectorySiteGroup_InsertedRowVersion] ON [track].[tblActiveDirectorySiteGroup]
(
    [InsertedRowVersion] ASC,
    [PK_ActiveDirectorySiteGroupGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectorySiteGroup_UpdatedRowVersion] ON [track].[tblActiveDirectorySiteGroup]
(
    [UpdatedRowVersion] ASC,
    [PK_ActiveDirectorySiteGroupGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectorySiteGroup_DeletedRowVersion] ON [track].[tblActiveDirectorySiteGroup]
(
    [DeletedRowVersion] ASC,
    [PK_ActiveDirectorySiteGroupGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectorySiteGroup_PK_ActiveDirectorySiteGroupGuid_Sync] ON [track].[tblActiveDirectorySiteGroup]
(
	[PK_ActiveDirectorySiteGroupGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblActiveDirectorySiteGroup_DeletedRowVersionUpdate_ForSync
   ON track.tblActiveDirectorySiteGroup
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
        FROM track.tblActiveDirectorySiteGroup t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END