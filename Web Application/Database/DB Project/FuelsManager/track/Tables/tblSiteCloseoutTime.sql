CREATE TABLE [track].[tblSiteCloseoutTime]
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
	[PK_SiteCloseoutTimeGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblSiteCloseoutTime_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblSiteCloseoutTime_PK_SiteCloseoutTimeGuid] ON [track].[tblSiteCloseoutTime]
(
	[PK_SiteCloseoutTimeGuid] ASC
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblSiteCloseoutTime_InsertedRowVersion] ON [track].[tblSiteCloseoutTime]
(
	[InsertedRowVersion] ASC,
	[PK_SiteCloseoutTimeGuid],
	[InsertedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblSiteCloseoutTime_UpdatedRowVersion] ON [track].[tblSiteCloseoutTime]
(
	[UpdatedRowVersion] ASC,
	[PK_SiteCloseoutTimeGuid],
	[UpdatedContext]
)
GO
 
CREATE NONCLUSTERED INDEX [IX_track_tblSiteCloseoutTime_DeletedRowVersion] ON [track].[tblSiteCloseoutTime]
(
	[DeletedRowVersion] ASC,
	[PK_SiteCloseoutTimeGuid],
	[DeletedContext]
)
GO
 
CREATE TRIGGER track.trg_insupd_tblSiteCloseoutTime_DeletedRowVersionUpdate_ForSync
   ON track.tblSiteCloseoutTime
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
        FROM track.tblSiteCloseoutTime t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END
GO
