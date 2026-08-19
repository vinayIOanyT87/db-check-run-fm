/* {CheckPoint: CREATING TRACKING TABLE for tblEntityUserToSite } */

/****** Object:  Table [track].[tblEntityUserToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityUserToSite]
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
	[PK_UserToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityUserToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityUserToSite_PK_UserToSiteGuid] ON [track].[tblEntityUserToSite]
(
    [PK_UserToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityUserToSite_InsertedRowVersion] ON [track].[tblEntityUserToSite]
(
    [InsertedRowVersion] ASC,
    [PK_UserToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityUserToSite_UpdatedRowVersion] ON [track].[tblEntityUserToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_UserToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityUserToSite_DeletedRowVersion] ON [track].[tblEntityUserToSite]
(
    [DeletedRowVersion] ASC,
    [PK_UserToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityUserToSite_PK_UserToSiteGuid_Sync] ON [track].[tblEntityUserToSite]
(
	[PK_UserToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityUserToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityUserToSite
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
        FROM track.tblEntityUserToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END