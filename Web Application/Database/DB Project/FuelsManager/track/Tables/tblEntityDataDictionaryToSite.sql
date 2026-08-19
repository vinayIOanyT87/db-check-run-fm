/* {CheckPoint: CREATING TRACKING TABLE for tblEntityDataDictionaryToSite } */

/****** Object:  Table [track].[tblEntityDataDictionaryToSite]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEntityDataDictionaryToSite]
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
	[PK_DataDictionaryToSiteGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEntityDataDictionaryToSite_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDataDictionaryToSite_PK_DataDictionaryToSiteGuid] ON [track].[tblEntityDataDictionaryToSite]
(
    [PK_DataDictionaryToSiteGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDataDictionaryToSite_InsertedRowVersion] ON [track].[tblEntityDataDictionaryToSite]
(
    [InsertedRowVersion] ASC,
    [PK_DataDictionaryToSiteGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDataDictionaryToSite_UpdatedRowVersion] ON [track].[tblEntityDataDictionaryToSite]
(
    [UpdatedRowVersion] ASC,
    [PK_DataDictionaryToSiteGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEntityDataDictionaryToSite_DeletedRowVersion] ON [track].[tblEntityDataDictionaryToSite]
(
    [DeletedRowVersion] ASC,
    [PK_DataDictionaryToSiteGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEntityDataDictionaryToSite_PK_DataDictionaryToSiteGuid_Sync] ON [track].[tblEntityDataDictionaryToSite]
(
	[PK_DataDictionaryToSiteGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEntityDataDictionaryToSite_DeletedRowVersionUpdate_ForSync
   ON track.tblEntityDataDictionaryToSite
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
        FROM track.tblEntityDataDictionaryToSite t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END