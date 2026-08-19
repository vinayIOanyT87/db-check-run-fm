/* {CheckPoint: CREATING TRACKING TABLE for tblActiveDirectoryUserGroup } */

/****** Object:  Table [track].[tblActiveDirectoryUserGroup]   Script Date: 8/28/2012 3:24:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectoryUserGroup]') AND type in (N'U'))
--BEGIN
--Creating Sync Tracking Table for tblActiveDirectoryUserGroup
CREATE TABLE [track].[tblActiveDirectoryUserGroup]
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
	[PK_ActiveDirectoryUserGroupGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblActiveDirectoryUserGroup_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
--END
GO


CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectoryUserGroup_PK_ActiveDirectoryUserGroupGuid] ON [track].[tblActiveDirectoryUserGroup]
(
    [PK_ActiveDirectoryUserGroupGuid],
    [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectoryUserGroup_InsertedRowVersion] ON [track].[tblActiveDirectoryUserGroup]
(
    [InsertedRowVersion] ASC,
    [PK_ActiveDirectoryUserGroupGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectoryUserGroup_UpdatedRowVersion] ON [track].[tblActiveDirectoryUserGroup]
(
    [UpdatedRowVersion] ASC,
    [PK_ActiveDirectoryUserGroupGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectoryUserGroup_DeletedRowVersion] ON [track].[tblActiveDirectoryUserGroup]
(
    [DeletedRowVersion] ASC,
    [PK_ActiveDirectoryUserGroupGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblActiveDirectoryUserGroup_PK_ActiveDirectoryUserGroupGuid_Sync] ON [track].[tblActiveDirectoryUserGroup]
(
	[PK_ActiveDirectoryUserGroupGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblActiveDirectoryUserGroup_DeletedRowVersionUpdate_ForSync
   ON track.tblActiveDirectoryUserGroup
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
        FROM track.tblActiveDirectoryUserGroup t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END